using System;
using System.Configuration;

namespace LeeraJenkins.Model
{
    public static class Settings
    {
        public static string Token => ConfigurationManager.AppSettings["BotApiKey"];
        public static string SpreadsheetId => ConfigurationManager.AppSettings["SpreadsheetId"];
        public static string SheetName => ConfigurationManager.AppSettings["SheetName"];
        public static string OfficialGamedaysSheetName => ConfigurationManager.AppSettings["OfficialGamedaysSheetName"];
        public static int SheetId => Convert.ToInt32(ConfigurationManager.AppSettings["SheetId"]);
        public static int RegularNotificationsStartingPeriodMin =>
            Convert.ToInt32(ConfigurationManager.AppSettings["RegularNotificationsStartingPeriodMin"]);
        public static int RegularNotificationsFinishingPeriodMin =>
            Convert.ToInt32(ConfigurationManager.AppSettings["RegularNotificationsFinishingPeriodMin"]);
        public static int FurthestGameRegistrationInMonth =>
            Convert.ToInt32(ConfigurationManager.AppSettings["FurthestGameRegistrationInMonth"]);
    }
}
