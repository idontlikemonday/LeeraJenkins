using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotConsole
{
    class Program
    {
        private static TelegramBotClient client;

        static void Main(string[] args)
        {
            var token = "966587803:AAEbCrzOiyNgcZxokPJ3PP-X8qkxI1pc1y0";

            var httpProxy = new WebProxy("217.21.146.105", 8080);

            client = new TelegramBotClient(token, httpProxy);
            client.StartReceiving();
            client.OnMessage += BotOnMessageReceived;

            Thread.Sleep(int.MaxValue);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
        }
    }
}
