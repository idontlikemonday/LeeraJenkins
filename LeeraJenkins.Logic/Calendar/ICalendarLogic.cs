using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LeeraJenkins.Logic.Calendar
{
    public interface ICalendarLogic
    {
        Task HandleCalendarCommand(CallbackQuery callbackQuery, string[] parameners);
    }
}
