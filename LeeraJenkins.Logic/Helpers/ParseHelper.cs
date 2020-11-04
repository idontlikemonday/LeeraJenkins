using System;
using System.Collections.Generic;
using System.Linq;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Logic.Extentions;
using LeeraJenkins.Model;
using LeeraJenkins.Resources;
using System.Text.RegularExpressions;
using System.Globalization;
using LeeraJenkins.Model.Dialog;

namespace LeeraJenkins.Logic.Helpers
{
    using Player = LeeraJenkins.Model.Core.Player;

    public static class ParseHelper
    {
        private static int _furthestGameRegistrationInMonth = Settings.FurthestGameRegistrationInMonth;
        private static string _playerFriendRegexp = Misc.PlayerFriendRegexp;

        public static GameRegistration FromTable(IList<object> row, string dateString, long rowNum)
        {
            try
            {
                AddDummyEmptyValues(row);

                var name = (string)row[(int)RegistrationTableColumnsOrder.Name];
                var timeString = (string)row[(int)RegistrationTableColumnsOrder.Time];
                var datetime = TryParseFullDatetimeWithLetteredMonth(dateString, timeString);

                if (datetime.HasValue)
                {
                    datetime = CheckNextYear(datetime.Value);
                }

                string durationFullString = ParseDurationString(row);

                var maxPlayers = TryParseMaxPlayers((string)row[(int)RegistrationTableColumnsOrder.MaxPlayers]);
                var playersNames = ParsePlayerNames(row, maxPlayers);

                return new GameRegistration()
                {
                    Guid = Guid.NewGuid(),
                    Date = datetime,
                    SheetRowId = rowNum,
                    Place = (string)row[(int)RegistrationTableColumnsOrder.Place],
                    Name = name,
                    Link = (string)row[(int)RegistrationTableColumnsOrder.Link],
                    Description = (string)row[(int)RegistrationTableColumnsOrder.Description],
                    Duration = durationFullString,
                    MaxPlayers = maxPlayers,
                    Host = FromString((string)row[(int)RegistrationTableColumnsOrder.Host]),
                    Players = playersNames.ToList(),
                    DateRaw = dateString,
                    TimeRaw = timeString
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static GameRegistration FromDialogSteps(List<DialogStep> steps, Player initiator)
        {
            var dateString = GetStringValueFromStep(steps, NewGameDialogStep.Date);
            var timeString = GetStringValueFromStep(steps, NewGameDialogStep.Time);
            var datetime = TryParseFullDatetimeWithGeneralFormat(dateString, timeString);
            var maxPlayers = TryParseMaxPlayers(GetStringValueFromStep(steps, NewGameDialogStep.MaxPlayers));
            if (maxPlayers == 0)
            {
                maxPlayers = 100;
            }
            var host = GetStringValueFromStep(steps, NewGameDialogStep.Host).ToLower() == "я буду хостом"
                ? initiator.ToString()
                : GetStringValueFromStep(steps, NewGameDialogStep.Host);
            var isPlayer = GetStringValueFromStep(steps, NewGameDialogStep.PlayerOrOnlyHost).ToLower() == "буду играть";

            var players = new List<Player>();
            if (isPlayer)
            {
                players.Add(initiator);
            }

            var gameReg = new GameRegistration()
            {
                Guid = Guid.NewGuid(),
                Date = datetime,
                Place = GetStringValueFromStep(steps, NewGameDialogStep.Place),
                Name = GetStringValueFromStep(steps, NewGameDialogStep.Name),
                Link = String.Empty,
                Description = String.Empty,
                Duration = GetStringValueFromStep(steps, NewGameDialogStep.Duration),
                MaxPlayers = maxPlayers,
                Host = ParseHelper.FromString(host),
                Players = players,
                DateRaw = dateString,
                TimeRaw = timeString
            };

            if (maxPlayers <= 10)
            {
                for (int i = gameReg.Players.Count; i < maxPlayers; i++)
                {
                    gameReg.Players.Add(new Player() { Name = "свободно" });
                }
            }

            return gameReg;
        }

        private static string GetStringValueFromStep(List<DialogStep> steps, NewGameDialogStep stepNum)
        {
            return steps[(int)stepNum].Value;
        }

        public static Player FromString(string fullname)
        {
            if (String.IsNullOrWhiteSpace(fullname))
            {
                return new Player() { Name = String.Empty, TgNickname = String.Empty };
            }

            var result = new Player();

            fullname = Regex.Replace(fullname, _playerFriendRegexp, @" $& ");
            var parts = Regex.Split(fullname, @"\s+");

            foreach (var part in parts)
            {
                if (!String.IsNullOrWhiteSpace(part))
                {
                    if (part.StartsWith("@"))
                    {
                        result.TgNickname = part;
                    }
                    else if (part.Contains("@"))
                    {
                        var namePart = part.Substring(0, part.IndexOf("@"));
                        result.Name = String.Join(" ", result.Name, namePart);
                        result.TgNickname = part.Substring(part.IndexOf("@"));
                    }
                    else
                    {
                        result.Name = String.Join(" ", result.Name, part);
                    }
                }
            }

            result.Name = result.Name == null ? null : result.Name.Trim();

            return result;
        }

        public static DateTime? TryParseDate(string datetime)
        {
            DateTime result;
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "dd.MM.yyyy", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "d.MM.yyyy", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        private static string ParseDurationString(IList<object> row)
        {
            var duration = (string)row[(int)RegistrationTableColumnsOrder.Duration];
            var maxDuration = TryParseMaxDuration(duration);
            var durationFullString = WordFormHelper.GetFullPluralPhrase(maxDuration,
                maxDuration < 10 ? PluralWordForms.Hours : PluralWordForms.Minutes,
                duration);
            return durationFullString;
        }

        private static IEnumerable<Player> ParsePlayerNames(IList<object> row, int? maxPlayers)
        {
            var playerObjects = row
                .Skip((int)RegistrationTableColumnsOrder.FirstPlayer)
                .Take((int)RegistrationTableColumnsOrder.LastPlayer - (int)RegistrationTableColumnsOrder.FirstPlayer)
                .ToList();

            playerObjects = TrimLastEmptyPlayers(playerObjects);

            if (maxPlayers.HasValue)
            {
                while (playerObjects.Count < maxPlayers.Value)
                {
                    playerObjects.Add(String.Empty);
                }
            }

            var players = playerObjects.Select(r => FromString((string)r)).ToList();

            if (maxPlayers.HasValue)
            {
                foreach (var player in players.Where(p => !p.IsEmpty).Skip(maxPlayers.Value))
                {
                    player.IsExcess = true;
                }
            }

            return players;
        }

        private static DateTime? TryParseFullDatetimeWithLetteredMonth(string dateString, string timeString)
        {
            var dateRegex = new Regex(@"(\d{1,2})\s+(\w+)");
            var timeRegex = new Regex(@"(\d{1,2}).{0,1}(\d{1,2})");
            var date = dateRegex.Match(dateString);
            var time = timeRegex.Match(timeString);
            var datetime = String.Format("{0} {1}:{2}", date.Value, time.Groups[1].Value, time.Groups[2].Value);

            DateTime result;
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "dd MMMM HH:mm", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "d MMMM HH:mm", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        private static DateTime? TryParseFullDatetimeWithGeneralFormat(string dateString, string timeString)
        {
            var datetime = String.Format("{0} {1}", dateString, timeString);

            DateTime result;
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "dd.MM.yyyy HH:mm", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }
            if (DateTime.TryParseExact(datetime.Trim().ToLower(), "d.MM.yyyy HH:mm", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        private static int? TryParseMaxPlayers(string maxPlayersString)
        {
            if (string.IsNullOrWhiteSpace(maxPlayersString))
            {
                return null;
            }
            int result = 0;
            if (int.TryParse(maxPlayersString, out result))
            {
                return result;
            }

            var maxPlayersRegex = new Regex(@"\d{1,2}");
            var maxPlayersMatches = maxPlayersRegex.Matches(maxPlayersString);
            result = GetLastMatch(maxPlayersMatches);
            return result;
        }

        private static int TryParseMaxDuration(string durationString)
        {
            int result = 0;
            if (int.TryParse(durationString, out result))
            {
                return result;
            }

            var maxDurationRegex = new Regex(@"\d{1,3}");
            var maxDurationMatches = maxDurationRegex.Matches(durationString);
            return GetLastMatch(maxDurationMatches);
        }

        private static int GetLastMatch(MatchCollection matches)
        {
            int result = 0;
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var value = match.Groups[match.Groups.Count - 1].Value;
                if (int.TryParse(value, out result))
                {
                    return result;
                }
            }

            return 0;
        }

        private static void AddDummyEmptyValues(IList<object> row)
        {
            while (row.Count <= (int)RegistrationTableColumnsOrder.AverageLastPlayer)
            {
                row.Add(string.Empty);
            };
        }

        private static DateTime CheckNextYear(DateTime datetime)
        {
            if (datetime < DateTime.Now)
            {
                if (datetime.AddMonths(_furthestGameRegistrationInMonth) < DateTime.Now)
                {
                    return datetime.AddYears(1);
                }
                return datetime;
            }
            return datetime;
        }

        private static List<object> TrimLastEmptyPlayers(List<object> playerObjects)
        {
            for (int i = playerObjects.Count - 1; i >= 0; i--)
            {
                if (String.IsNullOrEmpty((string)playerObjects[i]))
                {
                    playerObjects.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            return playerObjects;
        }
    }
}
