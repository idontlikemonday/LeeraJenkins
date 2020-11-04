using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using LeeraJenkins.Db;

namespace LeeraJenkins.DbRepository.Player
{
    using Player = LeeraJenkins.Db.Player;

    public class PlayerRepository : IPlayerRepository
    {
        public async Task<List<Player>> GetPlayersByGameId(long gameId)
        {
            using (var context = new TgEntities())
            {
                var players = context.GameRegistration
                    .SingleOrDefault(g => g.Id == gameId)
                    .GamePlayer.Select(gp => gp.Player)
                    .ToList();

                return players;
            }
        }

        public async Task UpdatePlayer(Player player)
        {
            using (var context = new TgEntities())
            {
                var players = context.Player;
                players.AddOrUpdate(player);

                context.SaveChanges();
            }
        }
    }
}
