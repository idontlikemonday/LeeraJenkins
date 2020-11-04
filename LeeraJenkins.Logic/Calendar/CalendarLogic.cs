using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Dispatchers;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.Sheet;
using LeeraJenkins.Logic.User;

namespace LeeraJenkins.Logic.Calendar
{
    public class CalendarLogic : ICalendarLogic
    {
        private IBotClient _botClient;
        private INewGameLogic _newGameLogic;
        private IUserLogic _userLogic;
        private IDialogMessageDispatcher _dialogDispatcher;
        private ISheetParser _sheetParser;

        public CalendarLogic(IBotClient botClient, INewGameLogic newGameLogic, IUserLogic userLogic, IDialogMessageDispatcher dialogDispatcher,
            ISheetParser sheetParser)
        {
            _botClient = botClient;
            _newGameLogic = newGameLogic;
            _userLogic = userLogic;
            _dialogDispatcher = dialogDispatcher;
            _sheetParser = sheetParser;
        }

        public async Task HandleCalendarCommand(CallbackQuery callbackQuery, string[] parameners)
        {
            if (parameners.Count() == 0)
            {
                return;
            }

            var client = _botClient.GetClient();

            var subcommand = parameners[0];
            var parameter = parameners[1];
            DateTime date;
            switch (subcommand)
            {
                case "Month":
                    date = DateTime.ParseExact(parameter, "MM.yyyy", new CultureInfo("ru-RU"));
                    var officialGamedays = _sheetParser.ParseOfficialGamedays();
                    var calendarMarkup = KeyboardMarkupHelper.GetCalendarKeyboardMarkup(date.Year, date.Month, officialGamedays);

                    var chatId = new ChatId(callbackQuery.Message.Chat.Id);
                    await client.EditMessageReplyMarkupAsync(chatId, callbackQuery.Message.MessageId, calendarMarkup);
                    break;
                default:
                    break;
            }
        }
    }
}
