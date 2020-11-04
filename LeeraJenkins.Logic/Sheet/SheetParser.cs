using System;
using System.Collections.Generic;
using System.Linq;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Model;

namespace LeeraJenkins.Logic.Sheet
{
    public class SheetParser : ISheetParser
    {
        private string _spreadsheetId = Settings.SpreadsheetId;
        private string _sheetName = Settings.SheetName;
        private string _officialGamedaysSheetName = Settings.OfficialGamedaysSheetName;
        private string _firstColumn = "A";
        private string _lastColumn = "Z";
        private int _startingRow = 2;

        private ISheetService _sheetService;

        public SheetParser(ISheetService sheetService)
        {
            _sheetService = sheetService;
        }

        public List<GameRegistration> ParseRegistrationTable(long? sheetRow = null, bool skipNoName = true)
        {
            bool isSingleGameParsing = sheetRow.HasValue;

            var range = isSingleGameParsing
                ? $"{_sheetName}!{_firstColumn}{_startingRow}:{_lastColumn}{sheetRow.Value}"
                : $"{_sheetName}!{_firstColumn}{_startingRow}:{_lastColumn}";

            var service = _sheetService.GetSheetService();
            IList<IList<object>> values = service.Spreadsheets.Values.Get(_spreadsheetId, range).Execute().Values;

            var gameRegs = new List<GameRegistration>();
            long rowNum = _startingRow;
            if (values != null && values.Count > 0)
            {
                var dateString = String.Empty;
                foreach (var row in values)
                {
                    if (row.Count > 0 && ((string)row[0]).Trim().ToLower().StartsWith("анонсы"))
                    {
                        break;
                    }
                    if (row.Count == 1)
                    {
                        dateString = (string)row.FirstOrDefault();
                    }
                    if (row.Count > 1)
                    {
                        var gameReg = ParseHelper.FromTable(row, dateString, rowNum);
                        if (gameReg != null)
                        {
                            gameRegs.Add(gameReg);
                        }
                    }
                    rowNum++;
                }
            }

            return gameRegs
                .Where(g => !skipNoName || skipNoName && !string.IsNullOrEmpty(g.Name))
                .ToList();
        }

        public List<DateTime> ParseOfficialGamedays()
        {
            var service = _sheetService.GetSheetService();
            var range = $"{_officialGamedaysSheetName}!A1:A1000";
            var result = new List<DateTime>();

            try
            {
                IList<IList<object>> values = service.Spreadsheets.Values.Get(_spreadsheetId, range).Execute().Values;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (row.Count == 1)
                        {
                            var dateString = ParseHelper.TryParseDate((string)row.FirstOrDefault());
                            if (dateString.HasValue)
                            {
                                result.Add(dateString.Value);
                            }
                        }
                    }
                }

                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
