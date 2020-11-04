using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201909252001)]
    public class Migration20190925TableLog : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("log").Table("Log").Exists())
            {
                Execute.Sql(@"
CREATE TABLE [log].[Log] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Date] [datetime] NOT NULL DEFAULT (getdate()),
    [Message] [nvarchar](max) NULL,
    [StackTrace] [nvarchar](max) NULL
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
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
