using System;
using System.Collections.Generic;
using LeeraJenkins.Model.Core;

namespace LeeraJenkins.Logic.Sheet
{
    public interface ISheetParser
    {
        List<GameRegistration> ParseRegistrationTable(long? sheetRow = null, bool skipNoName = true);
        List<DateTime> ParseOfficialGamedays();
    }
}
