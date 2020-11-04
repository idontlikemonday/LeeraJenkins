using System.Collections.Generic;
using System.Threading.Tasks;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Game
{
    public interface IGameLogic
    {
        Task SaveAllGamesRegistrations(IEnumerable<GameRegistration> gameList, bool truncate = false);
        Task<GameRegistration> GetGameBySheetRowId(long gameSheetId, string gameName = null);
        Task<DispatcherResult> RegisterPlayerToGame(Model.Core.Player player, long gameSheetId,
            string gameName = null, int? playerSlotNum = null);
        Task<DispatcherResult> DeRegisterPlayerToGame(Model.Core.Player player, long gameRowId,
            bool withFullGameDeletion, string trimmedGameName = null);
        Task UpdateFinishedGamesStatuses();
        bool CheckPlayerOrHostRegistration(GameRegistration gameReg, Model.Core.Player player);
        bool CheckRegistrationAsHost(GameRegistration gameReg, Model.Core.Player player);
        bool CheckRegistrationAsPlayer(GameRegistration gameReg, Model.Core.Player player);
        IEnumerable<GameRegistration> GetGameList(string playerName = null);
        IEnumerable<GameRegistration> GetVacantGameList(string playerName = null);
    }
}
