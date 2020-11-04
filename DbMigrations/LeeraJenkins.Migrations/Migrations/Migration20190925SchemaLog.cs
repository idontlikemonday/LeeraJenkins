using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201909252000)]
    public class Migration20190925SchemaLog : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("log").Exists())
            {
                Create.Schema("log");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
