using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202002191001)]
    public class Migration20200209TableUserDialog : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("UserDialog").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[UserDialog] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [UserId] [bigint] NOT NULL,
    [DialogId] [bigint] NOT NULL,
    [Date] [datetime] NOT NULL,
    CONSTRAINT [PK_UserDialog] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserDialog]
    ADD CONSTRAINT [FK_UserDialog_User_Id] FOREIGN KEY([UserId])
    REFERENCES [dbo].[User] ([Id])

ALTER TABLE [dbo].[UserDialog]
    ADD CONSTRAINT [FK_UserDialog_Dialog_Id] FOREIGN KEY([DialogId])
    REFERENCES [dbo].[Dialog] ([Id])
                ");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
