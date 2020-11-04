using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;
using LeeraJenkins.Model.Calendar;
using LeeraJenkins.Resources;
using LeeraJenkins.Logic.Sheet;

namespace LeeraJenkins.Logic.Helpers
{
    public static class KeyboardMarkupHelper
    {
        private static string _delimeter = Misc.CallbackDelimeter;

        private static ISheetParser _sheetParser;

        public static ReplyKeyboardMarkup GetDefaultKeyboardMarkup()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup(
                new List<List<KeyboardButton>>()
                {
                    new List<KeyboardButton>()
                    {
                        new KeyboardButton("Список игр"),
                        new KeyboardButton("Список свободных игр"),
                    },
                    new List<KeyboardButton>()
                    {
                        new KeyboardButton("Подробный список игр"),
                        new KeyboardButton("Подробный список свободных игр"),
                    },
                    new List<KeyboardButton>()
                    {
                        new KeyboardButton("📝 Новая игра [beta]"),
                    },
                    new List<KeyboardButton>()
                    {
                        new KeyboardButton("Мои ближайшие игры"),
                    }
                },
                resizeKeyboard: true
            );

            return keyboardMarkup;
        }

        public static InlineKeyboardMarkup GetCalendarKeyboardMarkup(int year, int month, List<DateTime> officialGamedays)
        {
            var result = new List<List<InlineKeyboardButton>>();

            var monthString = new DateTime(year, month, 1).ToString("MMMM", new CultureInfo("ru-RU"));
            var currMonth = new DateTime(year, month, 1).ToString("MM.yyyy", new CultureInfo("ru-RU"));
            var prevMonth = new DateTime(year, month, 1).AddMonths(-1).ToString("MM.yyyy", new CultureInfo("ru-RU"));
            var nextMonth = new DateTime(year, month, 1).AddMonths(1).ToString("MM.yyyy", new CultureInfo("ru-RU"));
            result.Add(new List<InlineKeyboardButton>()
            {
                { InlineKeyboardButton.WithCallbackData("⬅️", $"Calendar{_delimeter}Month{_delimeter}{prevMonth}") },
                { InlineKeyboardButton.WithCallbackData($"🗓 {monthString}", " ") },
                { InlineKeyboardButton.WithCallbackData("➡️", $"Calendar{_delimeter}Month{_delimeter}{nextMonth}") }
            });

            var elements = CalendarHelper.GetMonthCalendarElementsAsMatrix(year, month);
            foreach (var elementRow in elements)
            {
                var buttonRow = elementRow
                    .Select(e =>
                        {
                            var isSpecial = e.Day.HasValue && officialGamedays.Contains(e.Day.Value.Date);

                            return InlineKeyboardButton.WithCallbackData(
                                e.Day.HasValue
                                    ? (isSpecial ? $"🎲" : e.Day.Value.Day.ToString()) 
                                    : e.Value,
                                e.Day.HasValue
                                    ? $"Dialog{_delimeter}{e.Day.Value.ToString("dd.MM.yyyy")}"
                                    : " "
                            );
                        })
                    .ToList();
                result.Add(buttonRow);
            }

            return new InlineKeyboardMarkup(result);
        }

        public static InlineKeyboardMarkup GetInlineKeyboardMarkup(List<string> buttonCaptions, int maxButtonsInRow,
            List<DateTime> officialGamedays)
        {
            var result = new List<List<InlineKeyboardButton>>();
            var row = new List<InlineKeyboardButton>();

            if (buttonCaptions.Exists(b => b == "***Calendar***"))
            {
                var currentDate = DateTime.Now;
                return GetCalendarKeyboardMarkup(currentDate.Year, currentDate.Month, officialGamedays);
            }

            foreach (var buttonCaption in buttonCaptions)
            {
                if (row.Count == maxButtonsInRow)
                {
                    result.Add(row);
                    row = new List<InlineKeyboardButton>();
                }
                row.Add(InlineKeyboardButton.WithCallbackData(buttonCaption, $"Dialog{_delimeter}{buttonCaption}"));
            }
            if (row.Count > 0)
            {
                result.Add(row);
            }

            return new InlineKeyboardMarkup(result);
        }

        public static InlineKeyboardMarkup GetInlineKeyboardMarkup(List<InlineKeyboardButton> buttons, int maxButtonsInRow)
        {
            var result = new List<List<InlineKeyboardButton>>();
            var row = new List<InlineKeyboardButton>();

            foreach (var button in buttons)
            {
                if (button != null)
                {
                    if (row.Count == maxButtonsInRow)
                    {
                        result.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                    row.Add(button);
                }
            }
            if (row.Count > 0)
            {
                result.Add(row);
            }

            return new InlineKeyboardMarkup(result);
        }
    }
}
