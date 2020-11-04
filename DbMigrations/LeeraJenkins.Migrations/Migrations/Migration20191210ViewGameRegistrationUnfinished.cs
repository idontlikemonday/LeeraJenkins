using System;
using FluentMigrator;

namespace LeeraJenkins.Migrations.Migrations
{
    [Migration(201912101830)]
    public class Migration20191210ViewGameRegistrationUnfinished : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("log").Table("Log").Exists())
            {
                Execute.Sql(@"
BEGIN TRAN;
IF OBJECT_ID('dbo.GameRegistrationUnfinished', 'V') IS NOT NULL
	DROP VIEW dbo.GameRegistrationUnfinished;
GO

CREATE VIEW dbo.GameRegistrationUnfinished
AS
SELECT [Id]
      ,[Guid]
      ,[Name]
      ,[Date]
      ,[Place]
      ,[Link]
      ,[Description]
      ,[Duration]
      ,[MaxPlayers]
      ,[Status]
      ,[SheetRowId]
      ,[Created]
      ,[Host]
      ,[DateRaw]
      ,[TimeRaw]
FROM tg.dbo.GameRegistration
WHERE status = 0;

GO
COMMIT TRAN;
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
