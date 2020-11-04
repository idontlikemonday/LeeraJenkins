using System;
using System.Collections.Generic;
using Unity;
using LeeraJenkins.Logic.Commands;

namespace LeeraJenkins.Helpers
{
    public static class CommandHelper
    {
        public static List<ICommand> GetCommandList()
        {
            var container = UnityConfig.Container;

            return new List<ICommand>()
            {
                container.Resolve<StartCommand>(),
                container.Resolve<GamesListCommand>(),
                container.Resolve<GamesListDetailCommand>(),
                container.Resolve<VacantGamesListCommand>(),
                container.Resolve<VacantGamesListDetailCommand>(),
                container.Resolve<BookedGamesListCommand>(),
                container.Resolve<NewGameCommand>(),
                container.Resolve<DialogContinueCommand>(),
                container.Resolve<DialogPauseCommand>(),
                container.Resolve<DialogCancelCommand>(),
            };
        }

        public static List<ICommand> GetSpecialCommandList()
        {
            var container = UnityConfig.Container;

            return new List<ICommand>()
            {
                container.Resolve<PushCommand>()
            };
        }
    }
}
