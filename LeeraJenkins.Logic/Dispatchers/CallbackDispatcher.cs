using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Calendar;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Dispatchers
{
    using Player = LeeraJenkins.Model.Core.Player;

    public class CallbackDispatcher : ICallbackDispatcher
    {
        private IGameLogic _gameLogic;
        private ICalendarLogic _calednarLogic;
        private IDialogMessageDispatcher _dialogDispatcher;
        private IMapper _mapper;
        private IUserActionLogger _userActionLogger;
        private IBotClient _botClient;

        private string _delimeter = Misc.CallbackDelimeter;

        public CallbackDispatcher(IGameLogic gameLogic, ICalendarLogic calednarLogic, IDialogMessageDispatcher dialogDispatcher,
            IMapper mapper, IUserActionLogger userActionLogger, IBotClient botClient)
        {
            _gameLogic = gameLogic;
            _calednarLogic = calednarLogic;
            _dialogDispatcher = dialogDispatcher;
            _mapper = mapper;
            _userActionLogger = userActionLogger;
            _botClient = botClient;
        }

        public async Task<DispatcherResult> ExecuteDispatch(CallbackQuery callbackQuery)
        {
            var callbackString = callbackQuery.Data;
            if (String.IsNullOrWhiteSpace(callbackString))
            {
                return DispatcherResult.NoResult;
            }
            var parameters = callbackString.Split(new string[] { _delimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (parameters.Count() < 2)
            {
                return DispatcherResult.Error;
            }

            var command = parameters[0];
            parameters = parameters.Skip(1).ToArray();

            var player = _mapper.Map<Player>(callbackQuery.From);

            DispatcherResult result = DispatcherResult.NoResult;
            switch (command)
            {
                case "Registration":
                    result = await _gameLogic.RegisterPlayerToGame(player, Convert.ToInt64(parameters[0]), parameters[1]);
                    break;

                case "Deregistration":
                    result = await _gameLogic.DeRegisterPlayerToGame(player, Convert.ToInt64(parameters[0]), false, parameters[1]);
                    break;

                case "Delete":
                    result = await _gameLogic.DeRegisterPlayerToGame(player, Convert.ToInt64(parameters[0]), true, parameters[1]);
                    break;

                case "Calendar":
                    await _calednarLogic.HandleCalendarCommand(callbackQuery, parameters);
                    break;

                case "Dialog":
                    var client = _botClient.GetClient();
                    await client.AnswerCallbackQueryAsync(callbackQuery.Id, $"{parameters[0]}", showAlert: false);

                    var dialogDispatcherResult = await _dialogDispatcher.PerformDialogStep(callbackQuery.Message.Chat, parameters[0]);
                    result = _mapper.Map<DispatcherResult>(dialogDispatcherResult);
                    break;

                default:
                    break;
            }

            _userActionLogger.AddUserAction($"@{callbackQuery.From.Username}", callbackQuery.Data);

            return result;
        }
    }
}
