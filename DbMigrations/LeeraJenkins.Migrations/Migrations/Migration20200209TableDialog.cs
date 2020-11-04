using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(202002191000)]
    public class Migration20200209TableDialog : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("dbo").Table("Dialog").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [dbo].[Dialog] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    CONSTRAINT [PK_Dialog] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)
) ON [PRIMARY]
GO
                ");

                Execute.Sql(@"
SET IDENTITY_INSERT [dbo].[Dialog] ON;

INSERT INTO [dbo].[Dialog]
(Id, Name)
VALUES
(1, 'New game'),
(2, 'Friend registration')

SET IDENTITY_INSERT [dbo].[Dialog] OFF;
                ");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
