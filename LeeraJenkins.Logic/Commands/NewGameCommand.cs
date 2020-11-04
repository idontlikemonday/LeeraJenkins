using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Commands
{
    public class NewGameCommand : ICommand
    {
        private IDialogLogic _dialogLogic;
        private INewGameLogic _newGameLogic;
        private IUserLogic _userLogic;
        private IMapper _mapper;

        public string Name => "/newgame";
        public string Description => "Заведение строчки с новой игрой";

        public IList<string> Aliases => new List<string>() { "📝 Новая игра [beta]" };

        public NewGameCommand(IDialogLogic dialogLogic, INewGameLogic newGameLogic, IUserLogic userLogic, IMapper mapper)
        {
            _dialogLogic = dialogLogic;
            _newGameLogic = newGameLogic;
            _userLogic = userLogic;
            _mapper = mapper;
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var userId = await _userLogic.GetUserId(message.Chat.Id);

            if (userId == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Прежде чем создавать новую игру, тебе нужно зарегистрироваться с помощью команды /start");
                return;
            }

            var lastDialog = await _dialogLogic.GetLastActiveOrPausedDialog(userId.Value);
            if (lastDialog != null)
            {
                string messageText = String.Empty;
                if (lastDialog.Status == DialogStatus.Active)
                {
                    messageText =
                        $"У тебя активен диалог со мной прямо сейчас.{Environment.NewLine}{Environment.NewLine}" +
                        $"<i>" +
                        $"/cancel — завершить существующий диалог" +
                        $"</i>";
                }
                else
                {
                    messageText =
                        $"Уже есть незавершенный диалог со мной.{Environment.NewLine}{Environment.NewLine}" +
                        $"<i>" +
                        $"/continue — продолжить существующий диалог{Environment.NewLine}" +
                        $"/cancel — завершить существующий диалог" +
                        $"</i>";
                }
                await client.SendTextMessageAsync(message.Chat.Id, messageText, Telegram.Bot.Types.Enums.ParseMode.Html);
                return;
            }

            var userDialog = new UserDialog()
            {
                UserId = userId.Value,
                DialogId = DialogType.NewGame,
                Date = DateTime.Now,
                Status = DialogStatus.Active
            };
            var dialog = await _dialogLogic.InitiateDialog(userDialog);
            var initiator = _mapper.Map<Model.Core.Player>(message.From);
            var result = await _newGameLogic.GoToNextStep(userDialog, initiator);
        }
    }
}
