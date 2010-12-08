/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using log4net;
using Nini.Config;
using OpenMetaverse;
using OpenMetaverse.Messages.Linden;
using OpenMetaverse.Packets;
using OpenMetaverse.StructuredData;
using OpenSim.Framework;
using OpenSim.Framework.Servers;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using BlockingLLSDQueue = OpenSim.Framework.BlockingQueue<OpenMetaverse.StructuredData.OSD>;
using Aurora.Simulation.Base;
using OpenSim.Services.Interfaces;
using Caps = OpenSim.Framework.Capabilities.Caps;
using OpenSim.Framework.Capabilities;

namespace OpenSim.Services.Connectors
{
    public class EventQueueServicesConnector : EventQueueModuleBase, IEventQueueService, IService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private Dictionary<UUID, UUID> m_AvatarPasswordMap = new Dictionary<UUID, UUID>();
        private string m_serverURL = "";
        private ICapabilitiesModule m_capsModule = null;

        private void FindAndPopulateEQMPassword(UUID agentID)
        {
            if (m_capsModule != null)
            {
                Caps caps = m_capsModule.GetCapsHandlerForUser(agentID);
                if (caps != null)
                {
                    if (caps.RequestMap.ContainsKey("EventQueuePass"))
                    {
                        UUID Password = caps.RequestMap["EventQueuePass"].AsUUID();
                        m_AvatarPasswordMap[agentID] = Password;
                    }
                }
            }
        }

        #region IEventQueue Members

        public override bool Enqueue(OSD ev, UUID avatarID, ulong regionHandle)
        {
            //m_log.DebugFormat("[EVENTQUEUE]: Enqueuing event for {0} in region {1}", avatarID, m_scene.RegionInfo.RegionName);
            try
            {
                FindAndPopulateEQMPassword(avatarID);

                if (!m_AvatarPasswordMap.ContainsKey(avatarID))
                    return false;

                Dictionary<string, object> request = new Dictionary<string,object>();
                request.Add("AGENTID", avatarID.ToString());
                request.Add("REGIONHANDLE", regionHandle.ToString());
                request.Add("PASS", m_AvatarPasswordMap[avatarID].ToString());
                request.Add("LLSD", OSDParser.SerializeLLSDXmlString(ev));
                AsynchronousRestObjectRequester.MakeRequest("POST", m_serverURL, WebUtils.BuildQueryString(request));
            } 
            catch(Exception e)
            {
                m_log.Error("[EVENTQUEUE] Caught exception: " + e);
                return false;
            }
            
            return true;
        }

        public bool AuthenticateRequest(UUID agentID, UUID password, ulong regionHandle)
        {
            return true;
        }

        #endregion

        #region IService Members

        public string Name
        {
            get { return GetType().Name; }
        }

        public void Initialize(IConfigSource config, IRegistryCore registry)
        {
            IConfig handlerConfig = config.Configs["Handlers"];
            if (handlerConfig.GetString("EventQueueHandler", "") != Name)
                return;

            IConfig serviceConfig = config.Configs["EventQueueService"];
            if (serviceConfig != null)
            {
                string url = serviceConfig.GetString("EventQueueServiceURI");
                //Clean it up a bit
                url = url.EndsWith("/") ? url.Remove(url.Length - 1) : url;
                m_serverURL = url + "/CAPS/EQMPOSTER";
                registry.RegisterInterface<IEventQueueService>(this);
            }
        }

        public void PostInitialize(IRegistryCore registry)
        {
            m_capsModule = registry.Get<ICapabilitiesModule>();
        }

        #endregion
    }
}
