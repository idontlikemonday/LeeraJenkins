using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201909152000)]
    public class Migration20190915TableUser : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("User").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[User] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ChatId] bigint NOT NULL,
    [TelegramName] nvarchar(max) NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
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
