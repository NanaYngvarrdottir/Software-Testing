using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using Aurora.DataManager;
using Aurora.Framework;
using Aurora.Simulation.Base;
using Nini.Config;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using OpenSim.Services.Interfaces;
using StarDust.Collections.Interace;
using System.IO;

namespace StarDust.Collections
{
    public class ClassifiedsCollection : ConnectorBase, IStarDustCollector, IService
    {
        private bool m_enabled;
        private readonly Timer taskTimer = new Timer();
        IScheduleService m_scheduler;
        private IMoneyModule m_money;
        private bool m_DoClassifieds;
        private bool m_DoTier;
        private int m_TriesBeforeRemovingClassified;
        private bool m_NotifyOnClassifiedFailure;
        private bool m_NotifyOnClassifiedRemoval;
        private string m_NotifyType;
        private string m_EmailNotifyTemplateFileClassifiedFailure;
        private string m_EmailNotifyTemplateFileClassifiedRemoval;
        private string m_IMNotifyTemplateClassifiedFailure;
        private string m_IMNotifyTemplateClassifiedRemoval;
        private string m_EmailFormat;
        private bool m_NotifyOnClassifiedSuccess;
        private string m_EmailNotifyTemplateFileClassifiedSuccess;
        private string m_IMNotifyTemplateClassifiedSucess;
        private string m_SMTPHost;
        private bool m_SMTPUseCreditials;
        private string m_SMTPUser;
        private string m_SMTPPass;
        private string m_EmailFromAddress;
        private string m_EmailSubjectClassified;

        #region Implementation of IService

        public void Initialize(IConfigSource config, IRegistryCore registry)
        {
            IConfig configsection;
            if ((configsection = config.Configs["AuroraConnectors"]) != null)
                m_doRemoteCalls = configsection.GetBoolean("DoRemoteCalls", false);
            if (!CheckEnabled((m_doRemoteCalls) ? "Remote" : "Local", config)) return;
            

            IConfig economyConfig = config.Configs["Collections"];

            //;; Local for grid server, Remove for regions

            m_DoClassifieds = economyConfig.GetBoolean("DoClassifieds", false);
            m_DoTier = economyConfig.GetBoolean("DoTier", false);
	
            m_TriesBeforeRemovingClassified = economyConfig.GetInt("TriesBeforeRemovingClassified", 2);
            m_NotifyOnClassifiedFailure = economyConfig.GetBoolean("NotifyOnClassifiedFailure", false);
            m_NotifyOnClassifiedRemoval = economyConfig.GetBoolean("NotifyOnClassifiedRemoval", false);
            m_NotifyOnClassifiedSuccess = economyConfig.GetBoolean("NotifyOnClassifiedSuccess", false);
	
            //;; IM or Email
            m_NotifyType = economyConfig.GetString("NotifyType", "IM");
            //;; Used with Email
            m_EmailNotifyTemplateFileClassifiedFailure = economyConfig.GetString("EmailNotifyTemplateFileClassifiedFailure", "");
            m_EmailNotifyTemplateFileClassifiedRemoval = economyConfig.GetString("EmailNotifyTemplateFileClassifiedRemoval", "");
            m_EmailNotifyTemplateFileClassifiedSuccess = economyConfig.GetString("EmailNotifyTemplateFileClassifiedSuccess", "");

            m_EmailFormat = economyConfig.GetString("EmailFormat", "");
            //;; Used with IM
            m_IMNotifyTemplateClassifiedFailure = economyConfig.GetString("IMNotifyTemplateClassifiedFailure", "");
            m_IMNotifyTemplateClassifiedRemoval = economyConfig.GetString("IMNotifyTemplateClassifiedRemoval", "");
            m_IMNotifyTemplateClassifiedSucess = economyConfig.GetString("IMNotifyTemplateClassifiedSucess", "");

            if (m_NotifyType == "Email")
            {

                if ((!File.Exists(m_EmailNotifyTemplateFileClassifiedFailure)) ||
                    (!File.Exists(m_EmailNotifyTemplateFileClassifiedRemoval)) ||
                    (!File.Exists(m_EmailNotifyTemplateFileClassifiedSuccess)))
                {
                    MainConsole.Instance.Error(
                        "[StarDust.Collections]: Email Notification Template does not exist. Stardust.collection.Classifieds will be disabled");
                    m_enabled = false;
                    return;
                }

                m_EmailFromAddress = economyConfig.GetString("EmailFromAddress", "");
                if (m_EmailFromAddress == "")
                {
                    MainConsole.Instance.Error(
                        "[StarDust.Collections]: From email address is not filled in. Stardust.collection.Classifieds will be disabled");
                    m_enabled = false;
                    return;
                }

                m_SMTPHost = economyConfig.GetString("SMTPHost", "");
                if (m_SMTPHost == "")
                {
                    MainConsole.Instance.Error(
                        "[StarDust.Collections]: SMTPHost is not filled in. Stardust.collection.Classifieds will be disabled");
                    m_enabled = false;
                    return;
                }

                m_EmailSubjectClassified = economyConfig.GetString("EmailSubjectClassified", "");
                m_EmailFormat = economyConfig.GetString("EmailFormat", "Text");

                m_SMTPUseCreditials = economyConfig.GetBoolean("SMTPUseCreditials", true);
                m_SMTPUser = economyConfig.GetString("SMTPUser", "");
                m_SMTPPass = economyConfig.GetString("SMTPPass", "");

                if ((m_SMTPUseCreditials) && ((m_SMTPUser == "") || (m_SMTPPass == "")))
                {
                    MainConsole.Instance.Error(
                        "[StarDust.Collections]: No SMTP Creditials supplied. Stardust.collection.Classifieds will be disabled");
                    m_enabled = false;
                    return;
                }

                Smtp.smtpAddress = m_SMTPHost;
                Smtp.smtpUseCreditaials = m_SMTPUseCreditials;
                Smtp.smtpLogin = m_SMTPUser;
                Smtp.smtpPassword = m_SMTPPass;
            }



            Init(registry, "StardustCollector");
            m_registry.RegisterModuleInterface<IStarDustCollector>(this);
        }

        public void Start(IConfigSource config, IRegistryCore registry)
        {
            if (!m_enabled) return;
            m_scheduler = registry.RequestModuleInterface<IScheduleService>();
            m_money = registry.RequestModuleInterface<IMoneyModule>();
            

            m_scheduler.Register("CLASSBILL", Classified_Billing);
        }

        public void FinishedStartup()
        {
            if (!m_enabled) return;
            taskTimer.Interval = 120000;
            taskTimer.Elapsed += t_Elapsed;
            taskTimer.Enabled = m_enabled;
        }

        #endregion

        #region Billing Events

        private object Classified_Billing(string functionName, object parameters)
        {
            try
            {
                BillingClassified bc = new BillingClassified();
                bc.FromOSD((OSDMap)OSDParser.DeserializeJson(parameters.ToString()));
                IDirectoryServiceConnector DSC = DataManager.RequestPlugin<IDirectoryServiceConnector>();
                Classified c = DSC.GetClassifiedByID(bc.ClassifiedID);
                if (c != null)
                {
                    if (m_money != null)
                    {
                        if (!m_money.Charge(c.CreatorUUID, c.PriceForListing, "Classified Charge -" + c.Name))
                        {
                            bc.Tries += 1;
                            if (m_TriesBeforeRemovingClassified >= bc.Tries)
                            {
                                c.ClassifiedFlags =
                                    (byte) (c.ClassifiedFlags & ~((int) DirectoryManager.ClassifiedFlags.Enabled));
                                IProfileConnector profile =
                                    DataManager.RequestPlugin<IProfileConnector>("IProfileConnector");
                                profile.AddClassified(c);
                                m_scheduler.Remove(bc.ClassifiedID.ToString());
                                if (m_NotifyOnClassifiedRemoval)
                                {
                                    NotifyClass(c, bc, "Removal");
                                }
                            }
                            else
                            {
                                SchedulerItem si = m_scheduler.Get(c.ClassifiedUUID.ToString());
                                si.FireParams = bc.ToOSD();
                                m_scheduler.Save(si);
                                if (m_NotifyOnClassifiedFailure)
                                {
                                    NotifyClass(c, bc, "Failure");
                                }
                            }
                        }
                        else if (m_NotifyOnClassifiedSuccess)
                        {
                            NotifyClass(c, bc, "Success");
                        }
                    }
                    else
                        MainConsole.Instance.Info("[ClassifiedsCollection] Could not find money module.");
                }
                else
                    MainConsole.Instance.Info("[ClassifiedsCollection] Could not find classified, might have been deleted");
            }
            catch (Exception ex)
            {
                MainConsole.Instance.Error("[ClassifiedsCollection] Error charging for classifieds", ex);
            }

            return "";
        }

        private void NotifyClass(Classified classified, BillingClassified billingClassified, string typeOfNotice)
        {
            string NotifycationString = "";
            string NotifyTemplate = GetNotifyTemplate(typeOfNotice);
            if (NotifyTemplate == "") return;

           

            if (m_NotifyType == "Email")
            {
                
                IUserAccountService userService = m_registry.RequestModuleInterface<IUserAccountService>();
                UserAccount user = userService.GetUserAccount(new UUID(), classified.CreatorUUID);
                if (user != null)
                {
                    bool isHTML = (m_EmailFormat == "HTML");
                     NotifyTemplate = string.Format(NotifyTemplate, classified.Name, classified.PriceForListing, user.Name);
                     Smtp.Send(user.Email, m_EmailFromAddress, string.Format(m_EmailSubjectClassified, typeOfNotice), NotifyTemplate, isHTML, "normal");
                }
                else
                {
                    MainConsole.Instance.Error("[ClassifiedsCollection] Classified user does not exist");
                }
            }
            else
            {
                 NotifyTemplate = string.Format(NotifyTemplate, classified.Name, classified.PriceForListing);
                SendIM(classified.CreatorUUID, NotifyTemplate);
            }
        }

        private void SendIM(UUID towho, string message)
        {
            IGridWideMessageModule messageModule = m_registry.RequestModuleInterface<IGridWideMessageModule>();
            if (messageModule != null)
            {
                messageModule.MessageUser(towho, message);
                messageModule.MessageUser(towho, "/" + message);
            }    
        }

        private string GetNotifyTemplate(string typeOfNotice)
        {
            if (m_NotifyType == "Email")
            {
                switch (typeOfNotice)
                {
                    case "Removal":
                        return File.ReadAllText(m_EmailNotifyTemplateFileClassifiedRemoval);
                    case "Failure":
                        return File.ReadAllText(m_EmailNotifyTemplateFileClassifiedFailure);
                    case "Success":
                        return File.ReadAllText(m_EmailNotifyTemplateFileClassifiedSuccess);
                }
            }
            else
            {
                switch (typeOfNotice)
                {
                    case "Removal":
                        return m_IMNotifyTemplateClassifiedRemoval;
                    case "Failure":
                        return m_IMNotifyTemplateClassifiedFailure;
                    case "Success":
                        return m_IMNotifyTemplateClassifiedSucess;
                }
            }
            return "";
        }

        #endregion

        #region Timer

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_scheduler == null) return; // might be a long startup;
            taskTimer.Enabled = false;
            IDirectoryServiceConnector DSC = DataManager.RequestPlugin<IDirectoryServiceConnector>();

            int startAT = 0;
            bool keepGoing;
            do
            {
                keepGoing = false;
                List<DirClassifiedReplyData> classifieds = DSC.FindClassifieds("", ((int)DirectoryManager.ClassifiedCategories.Any).ToString(CultureInfo.InvariantCulture),
                                (uint)DirectoryManager.ClassifiedFlags.Enabled, startAT, UUID.Zero);
                startAT += classifieds.Count;
                if (classifieds.Count >= 1) keepGoing = true;
                foreach (DirClassifiedReplyData data in classifieds)
                {
                    if (m_scheduler.Exist(data.classifiedID.ToString())) continue;
                    SchedulerItem si = new SchedulerItem("CLASSBILL", OSDParser.SerializeJsonString(new BillingClassified(data.classifiedID).ToOSD()), false,
                                                         UnixTimeStampToDateTime((int)data.creationDate), 1,
                                                         RepeatType.months, data.classifiedID) { id = data.classifiedID.ToString() };
                    m_scheduler.Save(si);
                }
            } while (keepGoing);
        }

        #endregion

        #region private functions
        /// <summary>
        /// This is the same function used in stardust
        /// </summary>
        /// <param name="localOrRemote"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected bool CheckEnabled(string localOrRemote, IConfigSource source)
        {
            // check to see if it should be enabled and then load the config
            if (source == null) throw new ArgumentNullException("source");
            IConfig economyConfig = source.Configs["Collections"];
            m_enabled = (economyConfig != null)
                            ? (economyConfig.GetString("CollectionsConnector", "Remote") == localOrRemote)
                            : "Remote" == localOrRemote;
            return m_enabled;
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        #endregion
    }

    public class BillingClassified : IDataTransferable
    {
        #region initializer
        public BillingClassified(UUID classifiedID)
        {
            ClassifiedID = classifiedID;
        }

        public BillingClassified()
        {
        }

        #endregion
        

        #region IDataTransferable
        /// <summary>
        ///   Serialize the module to OSD
        /// </summary>
        /// <returns></returns>
        public override OSDMap ToOSD()
        {
            return new OSDMap()
                       {
                           {"ClassifiedID", OSD.FromUUID(ClassifiedID)},
                           {"Tries", OSD.FromInteger(Tries)}
                       };
        }

        /// <summary>
        ///   Deserialize the module from OSD
        /// </summary>
        /// <param name = "map"></param>
        public override void FromOSD(OSDMap map)
        {
            ClassifiedID = map["ClassifiedID"].AsUUID();
            if (map.Keys.Contains("Tries"))
                Tries = map["Tries"].AsInteger();
            else 
                Tries = 0;
        }
        #endregion

        #region properties

        public UUID ClassifiedID { get; set; }
        public int Tries { get; set; }

        #endregion
    }
}
