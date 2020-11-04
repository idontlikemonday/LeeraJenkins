using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201908250921)]
    public class Migration20190825TablePlayer : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("Player").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[Player] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(500) NULL,
    [TgName] nvarchar(500) NULL,
    CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
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
