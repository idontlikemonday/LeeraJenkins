using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeeraJenkins.Logic.Dialog
{
    public interface IPauseDialogLogic
    {
        Task<bool> PausePreviousDialogWithCheckingUserExistance(Message message, TelegramBotClient client);
    }
}
