using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201908262145)]
    public class Migration20190826TableGameRegistration : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("GameRegistration").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[GameRegistration] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Guid] uniqueidentifier NOT NULL DEFAULT (newid()),
    [Name] nvarchar(500) NOT NULL,
    [Date] datetime NULL,
    [Place] nvarchar(500) NOT NULL,
    [Link] nvarchar(500) NULL,
    [Description] nvarchar(max) NULL,
    [Duration] nvarchar(100) NULL,
    [MaxPlayers] nvarchar(100) NULL,
    [Status] int NULL,
    CONSTRAINT [PK_GameRegistration] PRIMARY KEY CLUSTERED 
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
