using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeraJenkins.Db;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.DbRepository.Game
{
    public class GameRepository : IGameRepository
    {
        public async Task<GameRegistration> CreateGame(GameRegistration game)
        {
            using (var context = new TgEntities())
            {
                var games = context.GameRegistration;
                games.Add(game);

                context.SaveChanges();

                return game;
            }
        }

        public async Task CreateGames(IList<GameRegistration> gameList)
        {
            using (var context = new TgEntities())
            {
                var games = context.GameRegistration;
                games.AddRange(gameList);

                context.SaveChanges();
            }
        }

        public async Task<IList<GameRegistration>> GetAllRegisteredGames()
        {
            using (var context = new TgEntities())
            {
                var games = context.GameRegistration
                    .Where(g => g.Status == 0);

                return games.ToList();
            }
        }

        public async Task<IList<GameRegistration>> GetAllRegisteredGamesWithIncludes()
        {
            using (var context = new TgEntities())
            {
                var games = context.GameRegistration
                    .Include(g => g.GamePlayer)
                    .Include(g => g.GamePlayer.Select(gp => gp.Player))
                    .Where(g => g.Status == 0);

                return games.ToList();
            }
        }

        public async Task<GameRegistration> GetByGuid(Guid id)
        {
            using (var context = new TgEntities())
            {
                var game = context.GameRegistration
                    .FirstOrDefault(g => g.Guid == id);

                return game;
            }
        }

        public async Task<GameRegistration> GetBySheetRowId(long sheetRowId, string gameName = null)
        {
            using (var context = new TgEntities())
            {
                var dbGame = context.GameRegistration
                    .Include(g => g.GamePlayer)
                    .Include(g => g.GamePlayer.Select(gp => gp.Player))
                    .Where(g => g.Status == 0)
                    .FirstOrDefault(g =>
                        g.SheetRowId == sheetRowId && (gameName == null || g.Name.StartsWith(gameName)));

                return dbGame;
            }
        }

        public async Task UpdateGamesStatuses(IEnumerable<GameRegistration> games, GameStatus status)
        {
            using (var context = new TgEntities())
            {
                foreach (var game in games)
                {
                    game.Status = (int)status;
                    context.GameRegistration.Attach(game);
                    context.Entry(game).Property(x => x.Status).IsModified = true;
                    context.SaveChanges();
                }
            }
        }

        public async Task DeleteAllRegisteredGames()
        {
            using (var context = new TgEntities())
            {
                var gamePlayers = context.GamePlayer
                    .Include(gp => gp.GameRegistration)
                    .Where(gp => gp.GameRegistration.Status == 0);

                var games = context.GameRegistration
                    .Where(g => g.Status == 0);

                var players = context.Player
                    .Include(p => p.GamePlayer)
                    .Include(p => p.GamePlayer.Select(gp => gp.GameRegistration))
                    .Where(p => gamePlayers.Select(gp => gp.Player.Id).Contains(p.Id));

                context.GamePlayer.RemoveRange(gamePlayers);
                context.GameRegistration.RemoveRange(games);
                context.Player.RemoveRange(players);

                context.SaveChanges();
            }
        }
    }
}
