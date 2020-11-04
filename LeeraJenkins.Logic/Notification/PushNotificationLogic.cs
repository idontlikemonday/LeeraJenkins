using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using LeeraJenkins.DbRepository.User;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Helpers;

namespace LeeraJenkins.Logic.Notification
{
    public class PushNotificationLogic : IPushNotificationLogic
    {
        private long _adminChatId = 397558545;
        private IBotClient _botClient;
        private IUserRepository _userRepository;

        public PushNotificationLogic(IBotClient botClient, IUserRepository userRepository)
        {
            _botClient = botClient;
            _userRepository = userRepository;
        }

        public async Task<int> SendMessageToAll(string message)
        {
            int recepientsSend = 0;
            var keyboardMarkup = KeyboardMarkupHelper.GetDefaultKeyboardMarkup();

            var client = _botClient.GetClient();
            var recepients = await _userRepository.GetAllUsers();

            foreach (var recepient in recepients)
            {
                try
                {
                    var chatId = recepient.ChatId;
                    await client.SendTextMessageAsync(chatId, message, ParseMode.Html,
                        replyMarkup: keyboardMarkup);
                    recepientsSend++;
                }
                catch (Exception ex)
                {
                    await client.SendTextMessageAsync(_adminChatId, $"Не удалось отправить {recepient.TelegramName}");
                }
            }

            return recepientsSend;
        }
    }
}
