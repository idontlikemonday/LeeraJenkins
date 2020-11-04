using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202001142000)]
    public class Migration20200114TableUserDateColumn : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("User").Column("Date").Exists())
            {
                Alter.Table("User").InSchema("dbo")
                    .AddColumn("Date").AsDateTime().Nullable();
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
