using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Extentions;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.Game;
using System.Linq;

namespace LeeraJenkins.Logic.Commands
{
    public class GamesListCommand : ICommand
    {
        protected IGameLogic _gameLogic;
        protected IPauseDialogLogic _pauseLogic;
        protected ILogger _logger;
        protected bool isFull;

        public GamesListCommand(IGameLogic gameLogic, IPauseDialogLogic pauseLogic, ILogger logger)
        {
            _gameLogic = gameLogic;
            _pauseLogic = pauseLogic;
            _logger = logger;
            isFull = false;
        }

        public virtual string Name => "/gameslist";

        public virtual string Description => "Список игр на ближайшие игровечера";

        public virtual IList<string> Aliases => new List<string>() { "Список игр" };

        public virtual async Task Execute(Message message, TelegramBotClient client)
        {
            var userExists = await _pauseLogic.PausePreviousDialogWithCheckingUserExistance(message, client);
            if (!userExists) { return; }

            var gameRegs = _gameLogic.GetGameList();
            if (gameRegs.Count() == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Ни одной игры в табличке! Пора изменять положение дел");
            }
            foreach (var gameReg in gameRegs)
            {
                try
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                        isFull ? gameReg.ToHtmlMessageString() : gameReg.ToHtmlLightMessageString(),
                        ParseMode.Html);
                }
                catch (Exception ex)
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                        "Тут должна была быть одна игра, но я не смогла ее вывести"
                    );

                    _logger.LogError($"Проблема отображения игры {gameReg.Name}", ex);
                }
            }
        }
    }
}
