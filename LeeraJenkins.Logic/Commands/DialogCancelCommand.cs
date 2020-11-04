using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.User;

namespace LeeraJenkins.Logic.Commands
{
    public class DialogCancelCommand : ICommand
    {
        private IDialogLogic _dialogLogic;
        private IUserLogic _userLogic;

        public string Name => "/cancel";
        public string Description => "Отмена существующего диалога";

        public IList<string> Aliases => new List<string>();

        public DialogCancelCommand(IDialogLogic dialogLogic, IUserLogic userLogic)
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

            var lastDialog = await _dialogLogic.GetLastActiveOrPausedDialog(userId.Value);
            if (lastDialog == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Нет диалогов для отмены");
                return;
            }
            await _dialogLogic.FinalizeUserDialog(lastDialog.Id);

            var dialogTypeFullForm = _dialogLogic.GetDialogTypeFullForm(lastDialog);
            await client.SendTextMessageAsync(message.Chat.Id, $"Я завершила твой последний диалог по {dialogTypeFullForm}");
        }
    }
}
