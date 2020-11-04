using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202001191000)]
    public class Migration20200119TableUserAction : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("log").Table("UserAction").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [log].[UserAction] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](max) NULL,
    [Action] [nvarchar](max) NULL,
    [Date] [datetime] NOT NULL DEFAULT (getdate())
    CONSTRAINT [PK_UserAction] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)
) ON [PRIMARY]
GO
                ");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
