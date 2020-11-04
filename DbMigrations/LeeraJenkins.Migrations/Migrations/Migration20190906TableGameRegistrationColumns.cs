using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201909061600)]
    public class Migration20190906TableGameRegistrationColumns : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("GameRegistration").Column("SheetRowId").Exists())
            {
                Alter.Table("GameRegistration").InSchema("dbo")
                    .AddColumn("SheetRowId").AsInt64().Nullable();
            }

            if (!Schema.Schema("dbo").Table("GameRegistration").Column("Created").Exists())
            {
                Alter.Table("GameRegistration").InSchema("dbo")
                    .AddColumn("Created").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now);
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
