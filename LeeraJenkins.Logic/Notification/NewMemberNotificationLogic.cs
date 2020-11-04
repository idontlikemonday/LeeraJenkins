using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Notification
{
    using Telegram.Bot.Types;

    public class NewMemberNotificationLogic : INewMemberNotificationLogic
    {
        private static List<string> _availableMessages;

        public async Task SayHi(TelegramBotClient client, User member, long chatId)
        {
            var messageTemplate = GetRandomMessageTemplateWithStrikeOut();
            var message = BuildMessage(messageTemplate,
                String.IsNullOrEmpty(member.Username) ? member.FirstName : $"@{member.Username}",
                Urls.TableUrl, Urls.FaqUrl);

            await Task.Factory.StartNew(() => WaitInSeconds(60, 120, client, chatId, message));
        }

        private async Task WaitInSeconds(int fromSecs, int toSecs, TelegramBotClient client, long chatId, string message)
        {
            Random r = new Random();
            var secs = r.Next(fromSecs, toSecs);

            await Task.Delay(secs * 1000);

            await client.SendTextMessageAsync(chatId, message, ParseMode.Html, disableWebPagePreview: true);
        }

        private string GetRandomMessageTemplateWithStrikeOut()
        {
            if (_availableMessages == null || _availableMessages.Count == 0)
            {
                _availableMessages = GetAllMessageTemplates().ToList();
            }

            Random r = new Random();
            var index = r.Next(_availableMessages.Count);
            var message = _availableMessages[index];

            _availableMessages.RemoveAt(index);

            return message;
        }

        private IEnumerable<string> GetAllMessageTemplates()
        {
            ResourceSet resourceSet = NewMemberMessages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                yield return entry.Value.ToString();
            }
        }

        private string BuildMessage(string messageTemplate, string memberName, string tableUrl, string faqUrl)
        {
            return String.Format(messageTemplate, memberName, tableUrl, faqUrl, Environment.NewLine);
        }
    }
}
