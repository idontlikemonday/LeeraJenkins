using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Dispatchers
{
    public interface ICallbackDispatcher
    {
        Task<DispatcherResult> ExecuteDispatch(CallbackQuery callback);
    }
}
