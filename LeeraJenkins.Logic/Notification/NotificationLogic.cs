using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Common.Helpers;
using LeeraJenkins.DbRepository.Game;
using LeeraJenkins.DbRepository.User;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.Sheet;
using LeeraJenkins.Model;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Notification
{
    public class NotificationLogic : INotificationLogic
    {
        private IBotClient _botClient;
        private IGameRepository _gameRepository;
        private IUserRepository _userRepository;
        private ISheetParser _sheetParser;
        private INotificationMessageGenerator _notificationMessageGenerator;
        private IMapper _mapper;

        private int _regularNotificationsStartingPeriodMin = Settings.RegularNotificationsStartingPeriodMin;
        private int _regularNotificationsFinishingPeriodMin = Settings.RegularNotificationsFinishingPeriodMin;

        public NotificationLogic(IBotClient botClient, IGameRepository gameRepository, IUserRepository userRepository,
            ISheetParser sheetParser, INotificationMessageGenerator notificationMessageGenerator, IMapper mapper)
        {
            _botClient = botClient;
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _sheetParser = sheetParser;
            _notificationMessageGenerator = notificationMessageGenerator;
            _mapper = mapper;
        }

        public async Task NotifyRegularDaily()
        {
            var client = _botClient.GetClient();

            var gamesForNotification = await GetGamesForRegularNotification();
            var allRecepientsFromGames = GetRecepientsTgNamesForRegularNotifications(gamesForNotification);

            foreach (var recepientTgName in allRecepientsFromGames)
            {
                var chatId = await _userRepository.GetChatId(recepientTgName);
                if (chatId != null && chatId > 0)
                {
                    var isPlayerAtLeastOnce = false;
                    var recepientMessages = new List<string>();
                    foreach (var game in gamesForNotification)
                    {
                        if (IsGameParticipant(game, recepientTgName))
                        {
                            bool isHost = IsGameHost(game, recepientTgName);
                            isPlayerAtLeastOnce = isPlayerAtLeastOnce || !isHost;
                            var notification = isHost
                                ? $"🤵🏻 — <b>{game.Name}</b> в <b>{game.Place}</b> в <b>{game.TimeRaw}</b>"
                                : $"🙎‍♂️ — <b>{game.Name}</b> в <b>{game.Place}</b> в <b>{game.TimeRaw}</b>. Хост — <b>{game.Host.ToString()}</b>";

                            recepientMessages.Add(notification);
                        }
                    }

                    var startMessage = "Привет! Не забудь про " + (recepientMessages.Count == 1 ? "свою завтрашнюю игру:" : "свои завтрашние игры:");
                    var endingMessage = isPlayerAtLeastOnce ? "Обязательно предупреди хоста, если не сможешь прийти на игру." : "";

                    var gamesString = String.Join(Environment.NewLine, recepientMessages);
                    var message = String.Join(Environment.NewLine + Environment.NewLine, startMessage, gamesString, endingMessage);
                    await client.SendTextMessageAsync(chatId, message, ParseMode.Html);
                }
            }
        }

        [Obsolete]
        public async Task NotifyRegularSeparate()
        {
            var gamesForNotification = await GetGamesForRegularNotification();

            var client = _botClient.GetClient();
            foreach (var game in gamesForNotification)
            {
                var recepients = GetRecepientsTgNames(game);
                foreach (var recepientTgName in recepients)
                {
                    var chatId = await _userRepository.GetChatId(recepientTgName);
                    if (chatId != null && chatId > 0)
                    {
                        bool isHost = IsGameHost(game, recepientTgName);

                        var notification = isHost
                            ? $"Уважаемый хост! Не забудь принести завтра свою игру <b>{game.Name}</b> в <b>{game.Place}</b> в <b>{game.TimeRaw}</b>!"
                            : $"Уважаемый игрок! Не забудь о своей завтрашней игре <b>{game.Name}</b> в <b>{game.Place}</b> в <b>{game.TimeRaw}</b>! " +
                              $"Обязательно предупреди хоста <b>{game?.Host?.ToString()}</b>, если не сможешь прийти на игру.";

                        await client.SendTextMessageAsync(chatId, notification, ParseMode.Html);
                    }
                }
            }
        }

        public async Task NotifyUsersWithChecking(NotificationPlayersModel model)
        {
            model.InitIntValues();

            var isSignleCellEdited = model.ColumnIndex == model.ColumnLastIndex && model.RowIndex == model.RowLastIndex;
            if (!isSignleCellEdited)
            {
                return;
            }

            var game = _sheetParser.ParseRegistrationTable(model.RowIndex)
                .FirstOrDefault(g => g.SheetRowId == model.RowIndex && g.Name.StartsWith(model.GameName));
            if (game == null || game.HasFinished())
            {
                return;
            }
            var isPlayerColumnEdited =
                model.ColumnIndex - 1 >= (int)RegistrationTableColumnsOrder.FirstPlayer
                && model.ColumnIndex - 1 <= (int)RegistrationTableColumnsOrder.LastPlayer;
            var isHostColumnEdited = model.ColumnIndex - 1 == (int)RegistrationTableColumnsOrder.Host;
            model = await UpdateNotificationModelValuesUsingDb(model, isPlayerColumnEdited, isHostColumnEdited);
            model.IsPlayerChangedNotification = isPlayerColumnEdited;
            model.IsHostChangedNotification = isHostColumnEdited;
            model.IsTimeChangedNotification = model.ColumnIndex - 1 == (int)RegistrationTableColumnsOrder.Time;

            await NotifyUsers(game, model);
        }

        public async Task NotifyUsers(GameRegistration game, NotificationBaseModel model = null,
            NotificationType notificationType = NotificationType.Unknown)
        {
            if (model == null)
            {
                model = new NotificationBaseModel();
            }
            if (game == null) return;
            if (game.HasFinished()) return;

            model.OldValue = model.OldValue?.Trim();
            model.Value = model.Value?.Trim();

            var recepients =
                GetRecepientsTgNames(game)
                .AddRecepientsFullNames(model.OldValue, model.Value);

            if (notificationType == NotificationType.Unknown)
            {
                if (!IsCellValueLogicallyChanged(model)) return;
                notificationType = GetNotificationType(model);
            }

            var client = _botClient.GetClient();

            foreach (var recepientFullName in recepients)
            {
                var chatId = await _userRepository.GetChatId(recepientFullName.GetTgName());
                if (chatId != null && chatId > 0)
                {
                    string message = _notificationMessageGenerator.GenerateRecepientNotificationMessage(
                        model, game, notificationType, recepientFullName);

                    if (!String.IsNullOrEmpty(message))
                    {
                        await client.SendTextMessageAsync(chatId, message, ParseMode.Html);
                    }
                }
            }
        }

        private static bool IsCellValueLogicallyChanged(NotificationBaseModel model)
        {
            bool valueChanged = true;
            if (model.OldValue == null && model.Value == null)
            {
                valueChanged = false;
            }
            if (model.OldValue.IsLogicallyEmptyAsPlayer()
                && model.Value.IsLogicallyEmptyAsPlayer())
            {
                return false;
            }
            if (model.OldValue != null && model.Value != null)
            {
                var playerOld = ParseHelper.FromString(model.OldValue);
                var playerCurrent = ParseHelper.FromString(model.Value);
                valueChanged = playerOld.GetRelationshipWith(playerCurrent) != PlayerRelationship.Same;
            }

            return valueChanged;
        }

        private NotificationType GetNotificationType(NotificationBaseModel model)
        {
            var isOldValueEmptySlot = model.OldValue.IsLogicallyEmptyAsPlayer();
            var isCurrentValueEmptySlot = model.Value.IsLogicallyEmptyAsPlayer();
            NotificationType editTableType = NotificationType.Unknown;

            if (model.IsHostChangedNotification)
            {
                return NotificationType.HostChanged;
            }
            if (model.IsTimeChangedNotification)
            {
                return NotificationType.TimeChanged;
            }
            if (model.IsPlayerChangedNotification)
            {
                if (!isCurrentValueEmptySlot && isOldValueEmptySlot)
                {
                    editTableType = NotificationType.PlayerRegistered;
                }
                if (!isCurrentValueEmptySlot && !isOldValueEmptySlot)
                {
                    editTableType = NotificationType.PlayerChanged;
                }
                if (isCurrentValueEmptySlot && !isOldValueEmptySlot)
                {
                    editTableType = NotificationType.PlayerDeleted;
                }
            }

            return editTableType;
        }

        private async Task TrySendMessageToExceedPlayer(NotificationBaseModel model, TelegramBotClient client, GameRegistration game)
        {
            var exceedPlayer = ParseHelper.FromString(model.Value);
            if (!String.IsNullOrWhiteSpace(exceedPlayer?.TgNickname))
            {
                var chatId = await _userRepository.GetChatId(exceedPlayer.TgNickname);
                if (chatId != null && chatId > 0)
                {
                    var message = $"⚠️ Внимание! Ты записался на игру <b>{game.Name}</b> с превышением максимального количества мест для записи!";
                    await client.SendTextMessageAsync(chatId, message, ParseMode.Html);
                }
            }
        }

        private async Task<List<GameRegistration>> GetGamesForRegularNotification()
        {
            var dbGames = await _gameRepository.GetAllRegisteredGamesWithIncludes();

            //var periodStart = DateTime.Now.AddMinutes(_regularNotificationsStartingPeriodMin);
            //var periodEnd = DateTime.Now.AddMinutes(_regularNotificationsFinishingPeriodMin);

            //var gamesForNotification = dbGames
            //    .Where(g => g.Date.HasValue
            //        && g.Date >= periodStart
            //        && g.Date < periodEnd)
            //    .Select(g => _mapper.Map<GameRegistration>(g))
            //    .ToList();

            var gamesForNotification = dbGames
                .Where(g => g.Date.HasValue
                    && g.Date >= DateTime.Now.Date.AddDays(1)
                    && g.Date < DateTime.Now.Date.AddDays(2))
                .Select(g => _mapper.Map<GameRegistration>(g))
                .ToList();

            return gamesForNotification;
        }

        private async Task<NotificationPlayersModel> UpdateNotificationModelValuesUsingDb(NotificationPlayersModel model,
            bool isPlayerColumnEdited, bool isHostColumnEdited)
        {
            var dbGame = await _gameRepository.GetBySheetRowId(model.RowIndex, model.GameName);
            if (dbGame == null)
            {
                return model;
            }
            var game = _mapper.Map<GameRegistration>(dbGame);

            if (isPlayerColumnEdited)
            {
                int editedPlayerIndex = model.ColumnIndex - (int)RegistrationTableColumnsOrder.FirstPlayer - 1;
                var isEditedExceededPlayer = editedPlayerIndex >= game.MaxPlayers;
                if (editedPlayerIndex < game.Players.Count)
                {
                    var editedPlayer = game.Players[editedPlayerIndex];
                    model.OldValue = editedPlayer.ToString();
                    editedPlayer.IsExcess = model.IsExceedPlayer = isEditedExceededPlayer;
                }
                else
                {
                    var excessPlayer = ParseHelper.FromString(model.Value);
                    excessPlayer.IsExcess = model.IsExceedPlayer = isEditedExceededPlayer;
                }
                return model;
            }
            if (isHostColumnEdited)
            {
                model.OldValue = game.Host.ToString();
            }

            return model;
        }

        private List<string> GetRecepientsTgNames(GameRegistration game, params string[] recepientExceptions)
        {
            return GetRecepientsFullNames(game, recepientExceptions)
                .Select(rec => rec.GetTgName())
                .Distinct()
                .ToList();
        }

        private List<string> GetRecepientsFullNames(GameRegistration game, params string[] recepientExceptions)
        {
            var result = new List<string>();

            result = game.Players
                .Where(p => !p.IsEmpty)
                .Where(p => !String.IsNullOrWhiteSpace(p.TgNickname))
                .Select(p => p.ToString())
                .ToList();
            var hostFullName = game.Host?.ToString();
            if (!String.IsNullOrEmpty(hostFullName))
            {
                result.Add(hostFullName);
            }

            var exceptionTgNames = recepientExceptions
                .Where(re => !String.IsNullOrWhiteSpace(re.GetTgName()))
                .Select(re => re)
                .Where(re => !String.IsNullOrWhiteSpace(re));

            result = result
                .Except(exceptionTgNames)
                .GetDistinctByTgName()
                .ToList();

            return result;
        }

        private List<string> GetRecepientsTgNamesForRegularNotifications(List<GameRegistration> games, params string[] recepientExceptions)
        {
            var recepientTgNames = new List<string>();

            recepientTgNames = games.SelectMany(g => g.Players)
                .Where(p => !p.IsEmpty)
                .Where(p => !String.IsNullOrWhiteSpace(p.TgNickname))
                .Select(p => p.TgNickname)
                .ToList();
            var hostTgNames = games
                .Select(g => g.Host?.TgNickname)
                .Where(host => !String.IsNullOrEmpty(host));
            recepientTgNames.AddRange(hostTgNames);

            var exceptionTgNames = recepientExceptions
                .Select(re => re?.GetTgName())
                .Where(re => !String.IsNullOrWhiteSpace(re));

            recepientTgNames = recepientTgNames
                .Except(exceptionTgNames)
                .Distinct()
                .ToList();

            return recepientTgNames;
        }

        private bool IsGameParticipant(GameRegistration game, string tgName, params string[] recepientExceptions)
        {
            return GetRecepientsTgNames(game, recepientExceptions)
                .Select(recepient => recepient?.ToLower()?.Trim())
                .Contains(tgName?.ToLower()?.Trim());
        }

        private bool IsGameHost(GameRegistration game, string tgName)
        {
            return !StringHelper.IsNotSame(game?.Host?.TgNickname?.GetTgName(), tgName);
        }
    }

    internal static class NotificationLogicExtentions
    {
        public static List<string> AddRecepientsFullNames(this List<string> recepientFullNames, params string[] recepientAdditions)
        {
            var additionsFullNames = recepientAdditions
                .Select(re => re?.GetTgName())
                .Where(re => !String.IsNullOrWhiteSpace(re));

            recepientFullNames.AddRange(additionsFullNames);

            recepientFullNames = recepientFullNames
                .GetDistinctByTgName()
                .ToList();

            return recepientFullNames;
        }

        public static List<string> GetDistinctByTgName(this IEnumerable<string> recepientFullNames)
        {
            var result = new List<string>();

            foreach (var recepient in recepientFullNames)
            {
                var recepientTgName = recepient.GetTgName();
                if (!String.IsNullOrEmpty(recepientTgName))
                {
                    if (!result.Select(r => r.GetTgName()).Contains(recepientTgName))
                    {
                        result.Add(recepient);
                    }
                }
            }

            return result;
        }
    }
}
