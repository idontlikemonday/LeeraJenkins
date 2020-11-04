using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.Registration;

namespace LeeraJenkins.Logic.Commands
{
    public class StartCommand : ICommand
    {
        private IRegistrationLogic _regLogic;

        public string Name => "/start";
        public string Description => "Стартовая команда";

        public IList<string> Aliases => new List<string>();

        public StartCommand(IRegistrationLogic regLogic)
        {
            _regLogic = regLogic;
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var user = new Model.Core.User()
            {
                ChatId = message.Chat.Id,
                TelegramName = $"@{message.From.Username}"
            };

            var result = await _regLogic.RegisterUser(user);

            var keyboardMarkup = KeyboardMarkupHelper.GetDefaultKeyboardMarkup();
            await client.SendTextMessageAsync(message.Chat.Id,
                string.Format(result.GetDescription(), message.From.Username),
                replyMarkup: keyboardMarkup);
        }
    }
}
