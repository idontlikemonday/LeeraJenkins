using System;
using System.Collections.Generic;

namespace LeeraJenkins.Logic.Sheet
{
    public interface ISheetWriter
    {
        void WriteValue(string value, string columnName, long sheetRowId);
        void WriteNewGameValues(List<string> newGameValues, long sheetRowId);
        void DeleteRow(long sheetRowId);
        void InsertNewDayRow(long sheetRowId, DateTime day);
        void InsertEmptyRow(long sheetRowId);
        void FormatNewGameRow(long sheetRowId);
    }
}
