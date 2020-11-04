using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using LeeraJenkins.Logic.Extentions;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Resources;
using System.Linq;
using System;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Helpers;

namespace LeeraJenkins.Logic.Commands
{
    public class BookedGamesListCommand : ICommand
    {
        private IGameLogic _gameLogic;
        private IPauseDialogLogic _pauseLogic;
        private ILogger _logger;
        protected string _delimeter = Misc.CallbackDelimeter;

        public BookedGamesListCommand(IGameLogic gameLogic, IPauseDialogLogic pauseLogic, ILogger logger)
        {
            _gameLogic = gameLogic;
            _pauseLogic = pauseLogic;
            _logger = logger;
        }

        public string Name => "/mygames";

        public string Description => "Список игр, на которые я записан";

        public IList<string> Aliases => new List<string>() { "Мои ближайшие игры" };

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var userExists = await _pauseLogic.PausePreviousDialogWithCheckingUserExistance(message, client);
            if (!userExists) { return; }

            var player = new Model.Core.Player()
            {
                Name = message.From.FirstName,
                TgNickname = $"@{message.From.Username}"
            };

            int bookedGamesCount = 0;
            var gameRegs = _gameLogic.GetGameList();
            foreach (var gameReg in gameRegs)
            {
                //var isRegistered = _gameLogic.CheckPlayerOrHostRegistration(gameReg, player);
                var isHost = _gameLogic.CheckRegistrationAsHost(gameReg, player);
                var isPlayer = _gameLogic.CheckRegistrationAsPlayer(gameReg, player);
                if (isHost || isPlayer)
                {
                    bookedGamesCount++;

                    InlineKeyboardButton hostButton = null;
                    InlineKeyboardButton playerButton = null;
                    if (isHost)
                    {
                        hostButton = InlineKeyboardButton.WithCallbackData(
                            $"❌ Полностью отменить игру \"{gameReg.Name}\"",
                            $"Delete{_delimeter}{gameReg.SheetRowId}{_delimeter}{gameReg.TrimmedName}");
                    }
                    if (isPlayer)
                    {
                        playerButton = InlineKeyboardButton.WithCallbackData(
                            $"➖ Освободить место на игру \"{gameReg.Name}\"",
                            $"Deregistration{_delimeter}{gameReg.SheetRowId}{_delimeter}{gameReg.TrimmedName}");
                    }

                    var inlineKeyboardMarkup = KeyboardMarkupHelper.GetInlineKeyboardMarkup(
                        new List<InlineKeyboardButton>() { playerButton, hostButton }, 1);
                    try
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                            gameReg.ToHtmlLightMessageString(),
                            ParseMode.Html,
                            replyMarkup: inlineKeyboardMarkup
                        );
                    }
                    catch (Exception ex)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                            "Тут должна была быть одна игра, но я не смогла ее вывести"
                        );

                        _logger.LogError($"Проблема отображения игры для отмены записи {gameReg.Name}", ex);
                    }
                }
            }

            if (bookedGamesCount == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Ты не записан ни на одну ближайшую игру. Попробуй записаться прямо сейчас: /vacantgameslist"
                );
            }
        }
    }
}
