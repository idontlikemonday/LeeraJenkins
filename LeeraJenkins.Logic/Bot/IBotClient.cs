using System.Threading.Tasks;
using Telegram.Bot;

namespace LeeraJenkins.Logic.Bot
{
    public interface IBotClient
    {
        TelegramBotClient GetClient();
    }
}
