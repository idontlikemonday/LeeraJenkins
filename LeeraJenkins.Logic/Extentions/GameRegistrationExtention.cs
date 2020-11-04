using System;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Resources;
using System.Linq;

namespace LeeraJenkins.Logic.Extentions
{
    public static class GameRegistrationExtention
    {
        public static string ToHtmlMessageString(this GameRegistration gameReg)
        {
            return string.Format(
                    "<b>Игра \"{2}\"</b>" +
                    "{9}{9}{0} в {1}" +
                    "{3}" +
                    "<i>{4}</i>" +
                    "{9}{9}Длительность {5}, максимум {6}" +
                    "{9}{9}<b>{10}</b>" +
                    "{9}{9}<b>Хостит</b> {7}{9}<b>Игроки:</b> {8}",
                gameReg.GetDateString(),
                gameReg.Place,
                gameReg.Name,
                gameReg.GetLinkString(true),
                gameReg.GetDescriptionString(true),
                gameReg.GetDurationString(),
                WordFormHelper.GetFullPluralPhrase(gameReg.MaxPlayersCalced, PluralWordForms.Players, gameReg.MaxPlayersString),
                gameReg.Host.ToString(),
                gameReg.GetNonEmptyPlayersList(),
                Environment.NewLine,
                gameReg.EmptySlots > 0
                    ? string.Format("Есть {0}", WordFormHelper.GetFullPluralPhrase(gameReg.EmptySlots, PluralWordForms.EmptySlots))
                    : "Нет свободных мест"
            );
        }

        public static string ToHtmlLightMessageString(this GameRegistration gameReg)
        {
            var dateString = gameReg.GetDateString();
            var descriptionString = gameReg.GetDescriptionString(true);
            var playersList = gameReg.GetNonEmptyPlayersList();
            var maxPlayersString = WordFormHelper.GetFullPluralPhrase(gameReg.MaxPlayersCalced, PluralWordForms.Players, gameReg.MaxPlayersString);

            return String.Format(
                    "<b>🎲 {2} 🎲</b>" +
                    "{4}{0} в {1}" +
                    "<i>{5}</i>" +
                    "{4}максимум {7}" +
                    "{4}<b>🤵🏻</b> {3}{4}<b>👪</b> {6}",
                dateString,
                gameReg.Place,
                gameReg.Name,
                gameReg.Host.ToString(),
                Environment.NewLine,
                descriptionString,
                playersList,
                maxPlayersString
            );
        }

        private static string GetRepeatedString(string value, int count)
        {
            var items = Enumerable.Repeat(value, count);
            return String.Join("", items);
        }
    }
}
