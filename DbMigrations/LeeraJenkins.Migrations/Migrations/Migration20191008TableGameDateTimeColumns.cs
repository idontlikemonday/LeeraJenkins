using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201910081900)]
    public class Migration20191008TableGameDateTimeColumns : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("GameRegistration").Column("DateRaw").Exists())
            {
                Alter.Table("GameRegistration").InSchema("dbo")
                    .AddColumn("DateRaw").AsString().Nullable();
            }

            if (!Schema.Schema("dbo").Table("GameRegistration").Column("TimeRaw").Exists())
            {
                Alter.Table("GameRegistration").InSchema("dbo")
                    .AddColumn("TimeRaw").AsString().Nullable();
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
