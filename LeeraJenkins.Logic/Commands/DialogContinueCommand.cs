using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.User;

namespace LeeraJenkins.Logic.Commands
{
    public class DialogContinueCommand : ICommand
    {
        private IDialogLogic _dialogLogic;
        private IUserLogic _userLogic;
        private INewGameLogic _newGameLogic;
        private IMapper _mapper;

        public string Name => "/continue";
        public string Description => "Продолжение последнего диалога";

        public IList<string> Aliases => new List<string>();

        public DialogContinueCommand(IDialogLogic dialogLogic, IUserLogic userLogic, INewGameLogic newGameLogic, IMapper mapper)
        {
            _dialogLogic = dialogLogic;
            _userLogic = userLogic;
            _newGameLogic = newGameLogic;
            _mapper = mapper;
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var userId = await _userLogic.GetUserId(message.Chat.Id);

            if (userId == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Сначала тебе нужно зарегистрироваться с помощью команды /start");
                return;
            }

            var lastDialog = await _dialogLogic.GetLastActiveOrPausedDialog(userId.Value);
            if (lastDialog == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Нет диалогов для продолжения");
                return;
            }
            await _dialogLogic.ContinueUserDialog(lastDialog.Id);
            await client.SendTextMessageAsync(message.Chat.Id, "Продолжаем прошлый диалог");

            var host = _mapper.Map<Model.Core.Player>(message.From);
            await _newGameLogic.GoToNextStep(lastDialog, host);
        }
    }
}
