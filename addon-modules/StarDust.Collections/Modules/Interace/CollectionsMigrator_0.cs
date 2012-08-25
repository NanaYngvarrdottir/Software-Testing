using System;
using System.Collections.Generic;
using Aurora.DataManager.Migration;
using C5;
using Aurora.Framework;

namespace StarDust.Currency.Grid.Dust
{
    public class CollectionsMigrator_0 : Migrator
    {
        public CollectionsMigrator_0()
        {
            Version = new Version(0, 0, 0);
            MigrationName = "Collections";

            //schema = new List<Rec<string, ColumnDefinition[], IndexDefinition[]>>();
            schema = new List<SchemaDefinition>();

            AddSchema("group_teir_donation", ColDefs(
                ColDef("avatar_id", ColumnTypes.String36),
                ColDef("group_id", ColumnTypes.String36),
                ColDef("teir", ColumnTypes.Integer30)
                ), IndexDefs(
                    IndexDef(new[] { "avatar_id", "group_id" }, IndexType.Primary)
                ));
        }

        protected override void DoCreateDefaults(IDataConnector genericData)
        {
            EnsureAllTablesInSchemaExist(genericData);
        }

        protected override bool DoValidate(IDataConnector genericData)
        {
            return TestThatAllTablesValidate(genericData);
        }

        protected override void DoMigrate(IDataConnector genericData)
        {
            DoCreateDefaults(genericData);
        }

        protected override void DoPrepareRestorePoint(IDataConnector genericData)
        {
            CopyAllTablesToTempVersions(genericData);
        }
    }
}