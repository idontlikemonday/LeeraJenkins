using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202002191002)]
    public class Migration20200209TableDialogStep : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("DialogStep").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[DialogStep] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [UserDialogId] [bigint] NOT NULL,
    [StepNum] [int] NOT NULL,
    [Value] [nvarchar](max) NULL,
    [Date] [datetime] NOT NULL,
    CONSTRAINT [PK_DialogStep] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DialogStep]
    ADD CONSTRAINT [FK_DialogStep_UserDialog_Id] FOREIGN KEY([UserDialogId])
    REFERENCES [dbo].[UserDialog] ([Id])
                ");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
