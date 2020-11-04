using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LeeraJenkins.Logic.Game;

namespace LeeraJenkins.Logic.Commands
{
    [Obsolete]
    public class SaveAllGamesCommand : ICommand
    {
        private IGameLogic _logic;

        public SaveAllGamesCommand(IGameLogic logic)
        {
            _logic = logic;
        }

        public string Name => "/save";
        public string Description => "Сохранить все записи на игры из таблички в БД";

        public IList<string> Aliases => new List<string>();

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var games = _logic.GetGameList();
            await _logic.SaveAllGamesRegistrations(games);
        }
    }
}
