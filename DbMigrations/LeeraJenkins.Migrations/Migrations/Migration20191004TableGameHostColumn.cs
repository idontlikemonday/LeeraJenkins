using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201910041900)]
    public class Migration20191004TableGameHostColumn : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("GameRegistration").Column("Host").Exists())
            {
                Alter.Table("GameRegistration").InSchema("dbo")
                    .AddColumn("Host").AsString().Nullable();
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
