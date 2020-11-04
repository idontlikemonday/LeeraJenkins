using System.Collections.Generic;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;

namespace LeeraJenkins.Logic.Commands
{
    public class GamesListDetailCommand : GamesListCommand
    {
        public GamesListDetailCommand(IGameLogic gameLogic, IPauseDialogLogic pauseLogic, ILogger logger)
            : base(gameLogic, pauseLogic, logger)
        {
            isFull = true;
        }

        public override string Name => "/gameslistdetail";

        public override string Description => "Подробный список игр на ближайшие игровечера";

        public override IList<string> Aliases => new List<string>() { "Подробный список игр" };
    }
}
