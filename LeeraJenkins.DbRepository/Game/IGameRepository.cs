using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeeraJenkins.Db;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.DbRepository.Game
{
    public interface IGameRepository
    {
        Task<GameRegistration> CreateGame(GameRegistration game);
        Task CreateGames(IList<GameRegistration> gameList);
        Task<IList<GameRegistration>> GetAllRegisteredGames();
        Task<IList<GameRegistration>> GetAllRegisteredGamesWithIncludes();
        Task<GameRegistration> GetByGuid(Guid id);
        Task<GameRegistration> GetBySheetRowId(long sheetRowId, string gameName = null);
        Task UpdateGamesStatuses(IEnumerable<GameRegistration> games, GameStatus status);
        Task DeleteAllRegisteredGames();
    }
}
