using System.Threading.Tasks;
using Telegram.Bot;

namespace LeeraJenkins.Logic.Notification
{
    using Telegram.Bot.Types;

    public interface INewMemberNotificationLogic
    {
        Task SayHi(TelegramBotClient client, User member, long chatId);
    }
}
