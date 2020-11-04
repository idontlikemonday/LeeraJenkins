using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.User;

namespace LeeraJenkins.Logic.Dialog
{
    public class PauseDialogLogic : IPauseDialogLogic
    {
        private IUserLogic _userLogic;
        private IDialogLogic _dialogLogic;

        public PauseDialogLogic(IUserLogic userLogic, IDialogLogic dialogLogic)
        {
            _userLogic = userLogic;
            _dialogLogic = dialogLogic;
        }

        public async Task<bool> PausePreviousDialogWithCheckingUserExistance(Message message, TelegramBotClient client)
        {
            var userId = await _userLogic.GetUserId($"@{message.From.Username}");
            if (userId == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Сначала тебе нужно зарегистрироваться с помощью команды /start");
                return false;
            }
            var lastDialog = await _dialogLogic.GetLastActiveDialog(userId.Value);
            if (lastDialog != null)
            {
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

            return true;
        }
    }
}
