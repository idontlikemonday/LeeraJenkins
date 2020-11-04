using AutoMapper;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Dispatchers
{
    public class DialogMessageDispatcher : IDialogMessageDispatcher
    {
        private IBotClient _botClient;
        private IDialogLogic _dialogLogic;
        private IUserLogic _userLogic;
        private INewGameLogic _newGameLogic;
        private IMapper _mapper;

        public DialogMessageDispatcher(IBotClient botClient, IDialogLogic dialogLogic, IUserLogic userLogic,
            INewGameLogic newGameLogic, IMapper mapper)
        {
            _botClient = botClient;
            _dialogLogic = dialogLogic;
            _userLogic = userLogic;
            _newGameLogic = newGameLogic;
            _mapper = mapper;
        }

        public async Task<DialogDispatcherResult> PerformDialogStep(Chat chat, string messageText)
        {
            var userId = await _userLogic.GetUserId(chat.Id);
            if (userId == null)
            {
                return DialogDispatcherResult.NullUser;
            }

            return await PerformDialogStep(chat, userId.Value, messageText);
        }

        public async Task<DialogDispatcherResult> PerformDialogStep(Chat chat, long userId, string stepValue)
        {
            var lastDialog = await _dialogLogic.GetLastActiveDialog(userId);
            if (lastDialog == null)
            {
                return DialogDispatcherResult.NoActiveDialogs;
            }

            var result = await _newGameLogic.ValidateAndAddNewStepWithValue(lastDialog, stepValue);
            if (result == DialogDispatcherResult.Success)
            {
                var host = _mapper.Map<Model.Core.Player>(chat);
                result = await _newGameLogic.GoToNextStep(lastDialog, host);
            }

            return result;
        }
    }
}
