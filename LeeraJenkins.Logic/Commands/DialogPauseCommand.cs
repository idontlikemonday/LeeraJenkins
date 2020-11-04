using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Commands
{
    public class DialogPauseCommand : ICommand
    {
        private IDialogLogic _dialogLogic;
        private IUserLogic _userLogic;

        public string Name => "/pause";
        public string Description => "Приостановка существующего диалога";

        public IList<string> Aliases => new List<string>();

        public DialogPauseCommand(IDialogLogic dialogLogic, IUserLogic userLogic)
        {
            _dialogLogic = dialogLogic;
            _userLogic = userLogic;
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var userId = await _userLogic.GetUserId(message.Chat.Id);

            if (userId == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Сначала тебе нужно зарегистрироваться с помощью команды /start");
                return;
            }

            var lastDialog = await _dialogLogic.GetLastActiveDialog(userId.Value);
            if (lastDialog == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Нет активных диалогов для приостановки");
                return;
            }
            await _dialogLogic.PauseUserDialog(lastDialog.Id);

            var dialogTypeFullForm = _dialogLogic.GetDialogTypeFullForm(lastDialog);
            await client.SendTextMessageAsync(message.Chat.Id,
                $"Я приостановила твой последний диалог по {dialogTypeFullForm}{Environment.NewLine}{Environment.NewLine}" +
                $"<i>" +
                $"/continue — продолжить диалог{Environment.NewLine}" +
                $"/cancel — окончательно завершить диалог" +
                $"</i>",
                Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}
