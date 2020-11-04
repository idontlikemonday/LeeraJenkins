using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Model.Core
{
    public class GameRegistration
    {
        public long Id { get; set; }
        public long SheetRowId { get; set; }
        public Guid Guid { get; set; }
        public DateTime? Date { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public int? MaxPlayers { get; set; }
        public string DateRaw { get; set; }
        public string TimeRaw { get; set; }
        public GameStatus Status { get; set; }

        public Player Host { get; set; }
        public List<Player> Players { get; set; }

        public bool IsRegisteredAsPlayer { get; set; }
        public bool IsRegisteredAsHost { get; set; }

        public int MaxPlayersCalced => MaxPlayers ?? Players?.Count ?? 0;
        public string MaxPlayersPostfix => MaxPlayers.HasValue ? string.Empty : "?";
        public string MaxPlayersString => $"{MaxPlayersCalced}{MaxPlayersPostfix}";
        public int EmptySlots => Players.Where(p => p.IsEmpty).Count();
        public string TrimmedName => new String(Name.Take(20).ToArray());
        public bool HasExcessPlayers => Players != null && Players.Count > MaxPlayers;
        public string DateString => GetDateString();

        public override string ToString()
        {
            return String.Format("Игра \"{2}\": ({0} в {1}) {3} {4} {5} {6}, хостит {7}, игроки: {8}",
                GetDateString(),
                Place, Name, Link, Description, Duration, MaxPlayers, Host.ToString(),
                String.Join(", ", Players.Select(p => String.Format("({0})", p.ToString()))));
        }

        public string GetDateString()
        {
            return Date.HasValue
                ? Date.Value.ToString("dd MMMM, dddd, HH:mm", CultureInfo.GetCultureInfo("ru-RU")).TrimStart('0')
                : $"{DateRaw} {TimeRaw}";
        }

        public string GetDateMessageString()
        {
            return Date.HasValue
                ? Date.Value.ToString("dddd, ", CultureInfo.GetCultureInfo("ru-RU")) + 
                  Date.Value.ToString("dd MMMM, в HH:mm", CultureInfo.GetCultureInfo("ru-RU")).TrimStart('0')
                : $"{DateRaw} {TimeRaw}";
        }

        public string GetNonEmptyPlayersList()
        {
            return string.Join(" │ ", Players.Where(p => !p.IsEmpty).Select(p => p.ToListedFormatString()));
        }

        public string GetLinkString(bool withNewline)
        {
            var optionalNewline = withNewline ? Environment.NewLine : String.Empty;
            return !String.IsNullOrWhiteSpace(Link) ? $"{optionalNewline}{Link}" : String.Empty;
        }

        public string GetDescriptionString(bool withNewline)
        {
            var optionalNewline = withNewline ? Environment.NewLine : String.Empty;
            return !String.IsNullOrWhiteSpace(Description) ? $"{optionalNewline}{Description}" : String.Empty;
        }

        public string GetDurationString()
        {
            return !String.IsNullOrWhiteSpace(Duration)
                ? Duration
                : "не указана";
        }

        public void MarkExcessPlayers()
        {
            foreach (var player in Players.Skip(MaxPlayersCalced))
            {
                player.IsExcess = true;
            }
        }

        public bool HasFinished()
        {
            if (Date == null || Date.Value < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
