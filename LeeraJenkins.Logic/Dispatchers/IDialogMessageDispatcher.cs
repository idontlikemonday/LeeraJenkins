using System.Threading.Tasks;
using Telegram.Bot.Types;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Dispatchers
{
    public interface IDialogMessageDispatcher
    {
        Task<DialogDispatcherResult> PerformDialogStep(Chat chat, string messageText);
        Task<DialogDispatcherResult> PerformDialogStep(Chat chat, long userId, string stepValue);
    }
}
