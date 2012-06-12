/*
 * This file's license:
 * 
 *  Copyright 2011 Matthew Beardmore
 *
 *  This file is part of Aurora.Addon.IRCChat.
 *  Aurora.Addon.IRCChat is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *  Aurora.Addon.IRCChat is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *  You should have received a copy of the GNU General Public License along with Aurora.Addon.IRCChat. If not, see http://www.gnu.org/licenses/.
 *
 * 
 * MetaBuilders.Irc.dll License:
 * 
 *  Microsoft Permissive License (Ms-PL)
 *  This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 *  1. Definitions
 *  The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
 *  A "contribution" is the original software, or any additions or changes to the software.
 *  A "contributor" is any person that distributes its contribution under this license.
 *  "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 *  2. Grant of Rights
 *  (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *  (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 *  3. Conditions and Limitations
 *  (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *  (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *  (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 *  (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *  (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aurora.Framework;
using OpenSim.Region.Framework.Interfaces;
using Nini.Config;
using MetaBuilders.Irc.Messages;
using MetaBuilders.Irc.Network;
using MetaBuilders.Irc;
using log4net;
using OpenMetaverse;

namespace Aurora.Addon.IRCChat
{
    public class IRCGroupService : INonSharedRegionModule
    {
        private static readonly ILog m_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<UUID, string> m_network = new Dictionary<UUID, string>();
        private Dictionary<UUID, string> m_channel = new Dictionary<UUID, string>();
        private Dictionary<UUID, string> m_gridName = new Dictionary<UUID, string>();
        private IScene m_scene;
        private bool m_spamDebug = false;
        private bool m_enabled = false;
        private Dictionary<UUID, Client> clients = new Dictionary<UUID,Client>();
        private IConfig m_config;

        public void Initialise (IConfigSource source)
        {
            IConfig ircConfig = source.Configs["IRCModule"];
            if(ircConfig != null)
            {
                m_enabled = ircConfig.GetBoolean("GroupsModule", m_enabled);
                m_spamDebug = ircConfig.GetBoolean("DebugMode", m_spamDebug);
                m_config = ircConfig;
            }
        }

        public void PostInitialise ()
        {
        }

        public void AddRegion (IScene scene)
        {
            if(!m_enabled)
                return;

            m_scene = scene;
            scene.EventManager.OnMakeRootAgent += EventManager_OnMakeRootAgent;
            scene.EventManager.OnMakeChildAgent += EventManager_OnMakeChildAgent;
            scene.EventManager.OnRemovePresence += EventManager_OnRemovePresence;
            scene.EventManager.OnIncomingInstantMessage += EventManager_OnIncomingInstantMessage;
            InitClients();
        }

        private void InitClients ()
        {
            IGroupsServicesConnector conn = m_scene.RequestModuleInterface<IGroupsServicesConnector>();
            foreach(string s in m_config.GetKeys())
            {
                if(s.EndsWith("_Network"))
                {
                    string networkvalue = m_config.GetString(s);
                    string channelvalue = m_config.GetString(s.Replace("_Network", "_Channel"));
                    string nickvalue = m_config.GetString(s.Replace("_Network", "_Nick"));
                    string gridName = m_config.GetString(s.Replace("_Network", "_GridName"), MainServer.Instance.ServerURI.Remove(0, 7));
                    GroupRecord g = conn.GetGroupRecord(UUID.Zero, UUID.Zero, s.Replace("_Network", "").Replace('_',' '));
                    if(g != null)
                    {
                        m_network[g.GroupID] = networkvalue;
                        m_channel[g.GroupID] = channelvalue;
                        m_gridName[g.GroupID] = gridName;
                        CreateIRCConnection(networkvalue, nickvalue, channelvalue, g.GroupID);
                    }
                }
            }
        }

        public void RegionLoaded (IScene scene)
        {
        }

        public void RemoveRegion (IScene scene)
        {
            if(!m_enabled)
                return;

            scene.EventManager.OnMakeRootAgent -= EventManager_OnMakeRootAgent;
            scene.EventManager.OnMakeChildAgent -= EventManager_OnMakeChildAgent;
            scene.EventManager.OnRemovePresence -= EventManager_OnRemovePresence;
            scene.EventManager.OnIncomingInstantMessage -= EventManager_OnIncomingInstantMessage;
        }

        public void Close ()
        {
        }

        public string Name
        {
            get { return "IRCParcelService"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        void EventManager_OnMakeRootAgent (IScenePresence presence)
        {
            presence.ControllingClient.OnPreSendInstantMessage += ControllingClient_OnPreSendInstantMessage;
        }

        void EventManager_OnRemovePresence (IScenePresence presence)
        {
            presence.ControllingClient.OnPreSendInstantMessage -= ControllingClient_OnPreSendInstantMessage;
        }

        void EventManager_OnMakeChildAgent (IScenePresence presence, OpenSim.Services.Interfaces.GridRegion destination)
        {
            presence.ControllingClient.OnPreSendInstantMessage -= ControllingClient_OnPreSendInstantMessage;
        }

        void EventManager_OnIncomingInstantMessage (GridInstantMessage message)
        {
            ControllingClient_OnPreSendInstantMessage(null, message);
        }

        bool ControllingClient_OnPreSendInstantMessage (IClientAPI remoteclient, GridInstantMessage im)
        {
            string name = remoteclient == null ? im.fromAgentName : remoteclient.Name;
            if(im.dialog == (byte)InstantMessageDialog.SessionSend)
            {
                Client client;
                if(clients.TryGetValue(im.imSessionID, out client))
                {
                    try
                    {
                        if(client.Connection.Status == ConnectionStatus.Connected)
                            client.SendChat("(grid:" + m_gridName[im.imSessionID] + ") " + name + ": " + im.message, m_channel[im.imSessionID]);
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }

        private void chatting (Object sender, IrcMessageEventArgs<TextMessage> e, UUID groupID)
        {
            IGroupsServicesConnector conn = m_scene.RequestModuleInterface<IGroupsServicesConnector>();
            IGroupsMessagingModule gMessaging = m_scene.RequestModuleInterface<IGroupsMessagingModule>();

            gMessaging.EnsureGroupChatIsStarted(groupID);
            gMessaging.SendMessageToGroup(new GridInstantMessage(null, UUID.Random(), e.Message.Sender.Nick,
                UUID.Zero, (byte)InstantMessageDialog.SessionSend, e.Message.Text, false, Vector3.Zero), groupID);
        }

        private void CreateIRCConnection (string network, string nick, string channel, UUID groupID)
        {
            // Create a new client to the given address with the given nick
            Client client = new Client(network, nick);
            Ident.Service.User = client.User;
            HookUpClientEvents(channel, groupID, client);
            client.EnableAutoIdent = false;
            client.Connection.Connect();
            clients[groupID] = client;
        }

        private void HookUpClientEvents (string channel, UUID groupID, Client client)
        {
            // Once I'm welcomed, I can start joining channels
            client.Messages.Welcome += delegate(Object sender, IrcMessageEventArgs<WelcomeMessage> e)
            {
                welcomed(sender, e, client, channel);
            };
            // People are chatting, pay attention so I can be a lame echobot :)
            client.Messages.Chat += delegate(Object sender, IrcMessageEventArgs<TextMessage> e)
            {
                chatting(sender, e, groupID);
            };

            client.Messages.TimeRequest += delegate(Object sender, IrcMessageEventArgs<TimeRequestMessage> e)
            {
                timeRequested(sender, e, client);
            };

            client.DataReceived += new EventHandler<ConnectionDataEventArgs>(dataGot);
            client.DataSent += new EventHandler<ConnectionDataEventArgs>(dataSent);

            client.Connection.Disconnected += new EventHandler<ConnectionDataEventArgs>(logDisconnected);
        }

        private void CloseClient (IScenePresence sp)
        {
            if(clients.ContainsKey(sp.UUID))
            {
                Client client = clients[sp.UUID];
                clients.Remove(sp.UUID);
                Util.FireAndForget(delegate(object o)
                {
                    client.SendQuit("Left the region");
                });
            }
        }

        private void logDisconnected (Object sender, ConnectionDataEventArgs e)
        {
            if(m_spamDebug)
            {
                String data = "*** Disconnected: " + e.Data;
                m_log.Warn("[RegionIRC]: " + data);
            }
        }

        private void dataGot (Object sender, ConnectionDataEventArgs e)
        {
            if(m_spamDebug)
            {
                String data = "*** Got: " + e.Data;
                m_log.Warn("[RegionIRC]: " + data);
            }
        }

        private void dataSent (Object sender, ConnectionDataEventArgs e)
        {
            if(m_spamDebug)
            {
                String data = "*** Sent: " + e.Data;
                m_log.Warn("[RegionIRC]: " + data);
            }
        }

        private void timeRequested (Object sender, IrcMessageEventArgs<TimeRequestMessage> e, Client client)
        {
            MetaBuilders.Irc.Messages.TimeReplyMessage reply = new MetaBuilders.Irc.Messages.TimeReplyMessage();
            reply.CurrentTime = DateTime.Now.ToLongTimeString();
            reply.Target = e.Message.Sender.Nick;
            client.Send(reply);
        }

        private void welcomed(Object sender, IrcMessageEventArgs<WelcomeMessage> e, Client client, string channel)
        {
            client.SendJoin(channel);
        }
    }
}
