using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeraJenkins.Logic.Player
{
    using Player = LeeraJenkins.Model.Core.Player;

    public class PlayerLogic : IPlayerLogic
    {
        private string _firstPlayerColumn = "I"; // 73

        public bool IsVacantPlaceExists(IList<Player> players, int? index = null)
        {
            var first = players.Skip(index ?? 0).FirstOrDefault(p => p.IsEmpty);
            return first != null;
        }

        public int GetFirstVacantPlaceIndex(IList<Player> players)
        {
            var index = players.IndexOf(players.FirstOrDefault(p => p.IsEmpty));
            return index;
        }

        public IList<Player> GetAddedPlayerToFirstVacantPlace(IList<Player> players, Player playerToAdd)
        {
            var index = GetFirstVacantPlaceIndex(players);
            players[index] = playerToAdd;

            return players;
        }

        public int GetPlayerPlaceIndex(IList<Player> players, Player player)
        {
            if (players == null || players.Count == 0)
            {
                return -1;
            }
            var selectedPlayer = players.FirstOrDefault(p => p.IsLogicallyEquals(player));
            if (player == null)
            {
                return -1;
            }

            var index = players.IndexOf(selectedPlayer);
            return index;
        }

        public string ConvertPlayerNumToColumnNum(int index)
        {
            var firstPlayerColumnAsciiCode = Encoding.ASCII.GetBytes(_firstPlayerColumn)[0];
            byte resultColumnAsciiCode = (byte)(firstPlayerColumnAsciiCode + index);

            var result = Encoding.ASCII.GetString(new byte[] { resultColumnAsciiCode });
            return result;
        }
    }
}
