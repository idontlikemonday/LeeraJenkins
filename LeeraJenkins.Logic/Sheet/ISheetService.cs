using System;
using Google.Apis.Sheets.v4;

namespace LeeraJenkins.Logic.Sheet
{
    public interface ISheetService
    {
        SheetsService GetSheetService();
    }
}
