using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeeraJenkins.Logic.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }

        IList<string> Aliases { get; }

        Task Execute(Message message, TelegramBotClient client);
    }
}
