using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201908271020)]
    public class Migration20190827TableGamePlayer : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("GamePlayer").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[GamePlayer] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [GameRegistrationId] bigint NOT NULL,
    [PlayerId] bigint NULL,
    [IsHost] bit NOT NULL DEFAULT (0),
    [PlayerNum] int NOT NULL,
    CONSTRAINT [PK_GamePlayer] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GamePlayer]
    ADD CONSTRAINT [FK_GamePlayer_Game_Id] FOREIGN KEY([GameRegistrationId])
    REFERENCES [dbo].[GameRegistration] ([Id])

ALTER TABLE [dbo].[GamePlayer]
    ADD CONSTRAINT [FK_GamePlayer_Player_Id] FOREIGN KEY([PlayerId])
    REFERENCES [dbo].[Player] ([Id])

                ");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
