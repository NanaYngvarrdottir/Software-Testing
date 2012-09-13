using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aurora.DataManager;
using Aurora.Framework;
using Nini.Config;
using StarDust.Collections.Interace;

namespace StarDust.Collections
{
    public class CollectorLocalConnector : IStarDustCollectorConnector
    {
        #region Implementation of IAuroraDataPlugin

        public string Name
        {
            get { return "IStarDustCollectorConnector"; }
        }

        public void Initialize(IGenericData GenericData, IConfigSource source, IRegistryCore simBase, string DefaultConnectionString)
        {
            DataManager.RegisterPlugin(Name, this);
        }

        #endregion
    }
}
