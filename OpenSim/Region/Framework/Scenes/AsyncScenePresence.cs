/*
 * Copyright (c) Contributors, http://virtualrealitygrid.org/, http://aurora-sim.org/, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Virtual Reality Project nor the
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenMetaverse;
using Aurora.Framework;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes.Animation;
using GridRegion = OpenSim.Services.Interfaces.GridRegion;
using OpenSim.Services.Interfaces;
using PrimType = Aurora.Framework.PrimType;

namespace OpenSim.Region.Framework.Scenes
{
    public class AsyncScenePresence : ScenePresence
    {
        #region Constructors

        public AsyncScenePresence()
        {
        }

        public AsyncScenePresence(IClientAPI client, IScene scene)
            :base(client, scene)
        {
        }

        #endregion

        #region Overridden Methods

        private readonly List<OUpdates> m_terseUpdates = new List<OUpdates>();

        public override void Update()
        {
            if ((Taints & PresenceTaint.ObjectUpdates) == PresenceTaint.ObjectUpdates)
            {
                Taints &= ~PresenceTaint.ObjectUpdates;

                lock (m_terseUpdates)
                {
                    foreach (OUpdates update in m_terseUpdates)
                        SceneViewer.QueuePartForUpdate(update.Child, update.Flags);
                    m_terseUpdates.Clear();
                }
            }
            base.Update();
        }

        #endregion

        #region Update Client(s)

        private class OUpdates
        {
            public ISceneChildEntity Child;
            public PrimUpdateFlags Flags;
        }

        /// <summary>
        /// Tell the SceneViewer for the given client about the update
        /// </summary>
        /// <param name="part"></param>
        /// <param name="flags"></param>
        public override void AddUpdateToAvatar(ISceneChildEntity part, PrimUpdateFlags flags)
        {
            lock (m_terseUpdates)
                m_terseUpdates.Add(new OUpdates {Child = part, Flags = flags});
            Scene.SceneGraph.TaintPresenceForUpdate(this, PresenceTaint.ObjectUpdates);
        }

        #endregion
    }
}
