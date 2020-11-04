using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Extentions;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Commands
{
    public class VacantGamesListCommand : ICommand
    {
        protected IGameLogic _gameLogic;
        protected IPauseDialogLogic _pauseLogic;
        protected ILogger _logger;
        protected bool isFull;
        protected string _delimeter = Misc.CallbackDelimeter;

        public VacantGamesListCommand(IGameLogic gameLogic, IPauseDialogLogic pauseLogic, ILogger logger)
        {
            _gameLogic = gameLogic;
            _pauseLogic = pauseLogic;
            _logger = logger;
            isFull = false;
        }

        public virtual string Name => "/vacantgameslist";

        public virtual string Description => "Список игр на ближайшие игровечера со свободными местами";

        public virtual IList<string> Aliases => new List<string>() { "Список свободных игр" };

        public virtual async Task Execute(Message message, TelegramBotClient client)
        {
            var userExists = await _pauseLogic.PausePreviousDialogWithCheckingUserExistance(message, client);
            if (!userExists) { return; }

            var gameRegs = _gameLogic.GetVacantGameList();
            var vacantGames = gameRegs.Where(g => g.EmptySlots > 0);
            if (vacantGames.Count() == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Игр со свободными местами пока нет. Ты всегда можешь создать запись на свою игру");
            }

            foreach (var gameReg in vacantGames)
            {
                var inlineButton = InlineKeyboardButton.WithCallbackData(
                    $"⬆️ Записаться на игру \"{gameReg.Name}\"",
                    $"Registration{_delimeter}{gameReg.SheetRowId}{_delimeter}{gameReg.TrimmedName}");

                try
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                        isFull ? gameReg.ToHtmlMessageString() : gameReg.ToHtmlLightMessageString(),
                        ParseMode.Html,
                        replyMarkup: new InlineKeyboardMarkup(inlineButton)
                    );
                }
                catch (Exception ex)
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                        "Тут должна была быть одна игра, но я не смогла ее вывести"
                    );

                    _logger.LogError($"Проблема отображения свободной игры {gameReg.Name}", ex);
                }
            }
        }
    }
}
