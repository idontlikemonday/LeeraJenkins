using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LeeraJenkins.DbRepository.Game;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.Notification;
using LeeraJenkins.Logic.Player;
using LeeraJenkins.Logic.Sheet;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;

namespace LeeraJenkins.Logic.Game
{
    public class GameLogic : IGameLogic
    {
        private IGameRepository _gameRepository;
        private IPlayerLogic _playerLogic;
        private ISheetParser _sheetParser;
        private ISheetWriter _sheetWriter;
        private INotificationLogic _notificationLogic;
        private IMapper _mapper;
        private ILogger _logger;

        public GameLogic(IGameRepository gameRepository, IPlayerLogic playerLogic, ISheetParser sheetParser,
            ISheetWriter sheetWriter, INotificationLogic notificationLogic, IMapper mapper, ILogger logger)
        {
            _gameRepository = gameRepository;
            _playerLogic = playerLogic;
            _sheetParser = sheetParser;
            _sheetWriter = sheetWriter;
            _notificationLogic = notificationLogic;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SaveAllGamesRegistrations(IEnumerable<GameRegistration> gameList, bool truncate = false)
        {
            var dbGameList = gameList
                .Select(g => _mapper.Map<Db.GameRegistration>(g))
                .ToList();

            if (truncate)
            {
                await _gameRepository.DeleteAllRegisteredGames();
            }

            await _gameRepository.CreateGames(dbGameList);
        }

        public async Task<GameRegistration> GetGameBySheetRowId(long gameSheetId, string gameName = null)
        {
            var dbGame = await _gameRepository.GetBySheetRowId(gameSheetId, gameName);

            if (dbGame == null)
            {
                return null;
            }

            var game = _mapper.Map<GameRegistration>(dbGame);
            return game;
        }

        public async Task<DispatcherResult> RegisterPlayerToGame(Model.Core.Player player, long gameRowId,
            string trimmedGameName = null, int? playerSlotNum = null)
        {
            try
            {
                var game = _sheetParser.ParseRegistrationTable(gameRowId)
                    .FirstOrDefault(g => g.SheetRowId == gameRowId && g.Name.StartsWith(trimmedGameName));
                if (game == null)
                {
                    return DispatcherResult.RegistrtingGameNotFound;
                }

                var players = game.Players;
                if (players.Any(p => p.IsLogicallyEquals(player)))
                {
                    return DispatcherResult.PlayerAlreadyRegistered;
                }
                if (!_playerLogic.IsVacantPlaceExists(players))
                {
                    return DispatcherResult.VacantPlaceNotFound;
                }
                if (!_playerLogic.IsVacantPlaceExists(players, playerSlotNum))
                {
                    playerSlotNum = null;
                }
                var index = playerSlotNum ?? _playerLogic.GetFirstVacantPlaceIndex(players);
                var columnName = _playerLogic.ConvertPlayerNumToColumnNum(index);

                _sheetWriter.WriteValue(player.ToString(), columnName, gameRowId);

                var notifyModel = new NotificationBaseModel
                {
                    OldValue = null,
                    Value = player.ToString(),
                    IsHostChangedNotification = false,
                    IsPlayerChangedNotification = true
                };
                await _notificationLogic.NotifyUsers(game, notifyModel);

                return DispatcherResult.SuccessfulGameRegistration;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка записи игрока на игру", ex);
                return DispatcherResult.GameRegistrationError;
            }
        }

        public async Task<DispatcherResult> DeRegisterPlayerToGame(Model.Core.Player player, long gameRowId,
            bool withFullGameDeletion, string trimmedGameName = null)
        {
            try
            {
                var game = _sheetParser.ParseRegistrationTable(gameRowId)
                    .FirstOrDefault(g => g.SheetRowId == gameRowId && g.Name.StartsWith(trimmedGameName));
                if (game == null)
                {
                    return DispatcherResult.DeRegistrtingGameNotFound;
                }

                var isHost = CheckRegistrationAsHost(game, player);
                var isPlayer = CheckRegistrationAsPlayer(game, player);
                if (!isHost && !isPlayer)
                {
                    return DispatcherResult.RegisteredPlaceNotFound;
                }
                if (isHost)
                {
                    if (withFullGameDeletion)
                    {
                        await DeleteGameRow(game);
                        return DispatcherResult.FullGameDeletion;
                    }
                    else
                    {
                        if (!isPlayer)
                        {
                            return DispatcherResult.CantDeRegisterHost;
                        }
                    }
                }

                //if (isHost && !isPlayer)
                //{
                //    if (!withFullGameDeletion)
                //    {
                //        return DispatcherResult.CantDeRegisterHost;
                //    }

                //    await DeleteGameRow(game);
                //    return DispatcherResult.FullGameDeletion;
                //}

                var index = _playerLogic.GetPlayerPlaceIndex(game.Players, player);
                var oldCellValue = index == -1 ? String.Empty : game.Players[index].Fullname;
                var columnName = _playerLogic.ConvertPlayerNumToColumnNum(index);

                Model.Core.Player tablePlayer = null;
                if (index <= game.Players.Count)
                {
                    tablePlayer = game.Players[index];
                }

                var newCellValue = tablePlayer != null && tablePlayer.IsExcess
                    ? ""
                    : "свободно";
                _sheetWriter.WriteValue(newCellValue, columnName, gameRowId);

                var notifyModel = new NotificationBaseModel
                {
                    OldValue = oldCellValue,
                    Value = newCellValue,
                    IsHostChangedNotification = false,
                    IsPlayerChangedNotification = true
                };
                await _notificationLogic.NotifyUsers(game, notifyModel);

                if (isHost && isPlayer)
                {
                    return DispatcherResult.HostAndPlayerDeRegisterWarning;
                }
                return DispatcherResult.SuccessfulGameDeRegistration;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка отмены записи игрока на игру", ex);
                return DispatcherResult.GameDeRegistrationError;
            }
        }

        public async Task UpdateFinishedGamesStatuses()
        {
            var dbGames = await _gameRepository.GetAllRegisteredGamesWithIncludes();
            var finishedGames = dbGames
                .Where(g => g.Date.HasValue && g.Date.Value <= DateTime.Now);

            await _gameRepository.UpdateGamesStatuses(finishedGames, GameStatus.Finished);
        }

        public bool CheckPlayerOrHostRegistration(GameRegistration gameReg, Model.Core.Player player)
        {
            var isHost = CheckRegistrationAsHost(gameReg, player);
            var isPlayer = CheckRegistrationAsPlayer(gameReg, player);

            return isHost || isPlayer;
        }

        public bool CheckRegistrationAsHost(GameRegistration gameReg, string playerName)
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                var player = ParseHelper.FromString(playerName);
                if (!player.IsEmpty)
                {
                    return CheckRegistrationAsHost(gameReg, player);
                }
            }

            return false;
        }

        public bool CheckRegistrationAsHost(GameRegistration gameReg, Model.Core.Player player)
        {
            if (gameReg?.Host?.TgNickname?.ToLower() == null)
            {
                return false;
            }
            if (gameReg.Host.IsLogicallyEquals(player))
            {
                return true;
            }
            return false;
        }

        public bool CheckRegistrationAsPlayer(GameRegistration gameReg, string playerName)
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                var player = ParseHelper.FromString(playerName);
                if (!player.IsEmpty)
                {
                    return CheckRegistrationAsPlayer(gameReg, player);
                }
            }

            return false;
        }

        public bool CheckRegistrationAsPlayer(GameRegistration gameReg, Model.Core.Player player)
        {
            if (gameReg.Players == null || gameReg.Players.Count == 0)
            {
                return false;
            }
            if (gameReg.Players.Any(p => p.IsLogicallyEquals(player)))
            {
                return true;
            }
            return false;
        }

        public IEnumerable<GameRegistration> GetGameList(string playerName = null)
        {
            var games = _sheetParser.ParseRegistrationTable();
            MarkGamesForBeingHostAndPlayer(games, playerName);
            return games;
        }

        public IEnumerable<GameRegistration> GetVacantGameList(string playerName = null)
        {
            var games = GetGameList(playerName);
            return games.Where(g => g.EmptySlots > 0);
        }

        private async Task DeleteGameRow(GameRegistration game)
        {
            _sheetWriter.DeleteRow(game.SheetRowId);

            await _notificationLogic.NotifyUsers(game, null, NotificationType.GameDeleted);
        }

        private void MarkGamesForBeingHostAndPlayer(List<GameRegistration> games, string playerName)
        {
            foreach (var game in games)
            {
                game.IsRegisteredAsHost = CheckRegistrationAsHost(game, playerName);
                game.IsRegisteredAsPlayer = CheckRegistrationAsPlayer(game, playerName);
            }
        }
    }
}
