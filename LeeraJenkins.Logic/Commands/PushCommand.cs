using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Notification;

namespace LeeraJenkins.Logic.Commands
{
    public class PushCommand : ICommand
    {
        private long _adminChatId = 397558545;
        private IPushNotificationLogic _pushLogic;

        public string Name => "/push";
        public string Description => "Push-уведомление";

        public IList<string> Aliases => new List<string>();

        public PushCommand(IPushNotificationLogic pushLogic)
        {
            _pushLogic = pushLogic;
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            if (message.Chat.Id == _adminChatId)
            {
                var messageToSend = message.Text.Substring(Name.Length);
                var recepientsSend = await _pushLogic.SendMessageToAll(messageToSend);

                await client.SendTextMessageAsync(_adminChatId, $"Количество получателей: {recepientsSend}");
            }
        }
    }
}
