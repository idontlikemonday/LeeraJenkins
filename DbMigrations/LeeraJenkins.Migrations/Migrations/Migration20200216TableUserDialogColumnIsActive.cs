using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202002201000)]
    public class Migration20200216TableUserDialogColumnIsActive : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("UserDialog").Column("IsActive").Exists())
            {
                Alter.Table("UserDialog").InSchema("dbo")
                    .AddColumn("IsActive").AsBoolean().NotNullable();
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
