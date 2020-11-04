using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeeraJenkins.DbRepository.Player
{
    using Player = LeeraJenkins.Db.Player;

    public interface IPlayerRepository
    {
        Task<List<Player>> GetPlayersByGameId(long gameId);
        Task UpdatePlayer(Player player);
    }
}
