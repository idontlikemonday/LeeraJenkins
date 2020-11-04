using Telegram.Bot;
using LeeraJenkins.Model;

namespace LeeraJenkins.Logic.Bot
{
    public class BotClient : IBotClient
    {
        private static TelegramBotClient _client;

        public TelegramBotClient GetClient()
        {
            _client = new TelegramBotClient(Settings.Token);

            return _client;
        }
    }
}
