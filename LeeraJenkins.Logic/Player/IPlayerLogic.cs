using System.Collections.Generic;

namespace LeeraJenkins.Logic.Player
{
    using Player = LeeraJenkins.Model.Core.Player;

    public interface IPlayerLogic
    {
        bool IsVacantPlaceExists(IList<Player> players, int? index = null);
        int GetFirstVacantPlaceIndex(IList<Player> players);
        int GetPlayerPlaceIndex(IList<Player> players, Player player);
        IList<Player> GetAddedPlayerToFirstVacantPlace(IList<Player> players, Player playerToAdd);
        string ConvertPlayerNumToColumnNum(int index);
    }
}
