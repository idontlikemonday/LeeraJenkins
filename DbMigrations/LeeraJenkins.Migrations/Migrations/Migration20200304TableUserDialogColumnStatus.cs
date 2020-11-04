using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202003042300)]
    public class Migration20200304TableUserDialogColumnStatus : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("UserDialog").Column("Status").Exists())
            {
                Alter.Table("UserDialog").InSchema("dbo")
                    .AddColumn("Status").AsInt32().Nullable();

                Execute.Sql("UPDATE dbo.UserDialog SET Status = 2");

                Alter.Column("Status").OnTable("UserDialog").InSchema("dbo")
                    .AsInt32().NotNullable();
            }

            if (Schema.Schema("dbo").Table("UserDialog").Column("IsActive").Exists())
            {
                Delete.Column("IsActive").FromTable("UserDialog").InSchema("dbo");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
