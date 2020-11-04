using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using LeeraJenkins.Common.Helpers;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.Notification;
using LeeraJenkins.Logic.Sheet;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.NewGame
{
    public class NewGameLogic : INewGameLogic
    {
        private IBotClient _botClient;
        private IDialogLogic _dialogLogic;
        private IUserLogic _userLogic;
        private INotificationLogic _notificationLogic;
        private ISheetParser _sheetParser;
        private ISheetWriter _sheetWriter;

        private List<DialogStepAdditional> _steps;

        public NewGameLogic(IBotClient botClient, IDialogLogic dialogLogic, IUserLogic userLogic,
            INotificationLogic notificationLogic, ISheetParser sheetParser, ISheetWriter sheetWriter)
        {
            _botClient = botClient;
            _dialogLogic = dialogLogic;
            _userLogic = userLogic;
            _notificationLogic = notificationLogic;
            _sheetParser = sheetParser;
            _sheetWriter = sheetWriter;

            InitStepMessagesAndValidations();
        }

        public async Task<DialogDispatcherResult> ValidateAndAddNewStepWithValue(UserDialog lastDialog, string value)
        {
            var userId = lastDialog.UserId;
            var currentStepNum = await GetCurrentStepNum(userId);

            var isValid = ValidateStepValue(value, currentStepNum);
            if (!isValid)
            {
                return DialogDispatcherResult.ValidationError;
            }
            isValid = ValidateStepValueAdditional(value, currentStepNum);
            if (!isValid)
            {
                return DialogDispatcherResult.DateFromPast;
            }

            var newStep = new DialogStep()
            {
                StepNum = currentStepNum + 1,
                UserDialogId = lastDialog.Id,
                Value = value,
                Date = DateTime.Now
            };
            await _dialogLogic.AddStep(newStep);

            return DialogDispatcherResult.Success;
        }

        public async Task<DialogDispatcherResult> GoToNextStep(UserDialog lastDialog, Model.Core.Player initiator)
        {
            var userId = lastDialog.UserId;
            var currentStepNum = await GetCurrentStepNum(userId);

            var user = await _userLogic.GetUser(userId);
            if (user == null)
            {
                return DialogDispatcherResult.NullUser;
            }

            var chatId = user.ChatId;
            if (currentStepNum >= _steps.Count)
            {
                return await FinalizeDialog(lastDialog.Id, initiator);
            }

            await SendStepMessage(chatId, currentStepNum);
            return DialogDispatcherResult.Success;
        }

        public async Task<DialogDispatcherResult> FinalizeDialog(long userDialogId, Model.Core.Player initiator)
        {
            var gameReg = await TryCreateGameRegistration(userDialogId, initiator);
            await _notificationLogic.NotifyUsers(gameReg, null, NotificationType.GameCreated);
            await _dialogLogic.FinalizeUserDialog(userDialogId);
            return DialogDispatcherResult.Finalized;
        }

        private async Task<GameRegistration> TryCreateGameRegistration(long userDialogId, Model.Core.Player initiator)
        {
            var steps = await _dialogLogic.GetDialogSteps(userDialogId);
            var gameReg = ParseHelper.FromDialogSteps(steps, initiator);
            if (gameReg.Date.HasValue)
            {
                var games = _sheetParser.ParseRegistrationTable(skipNoName: false);

                var sheetRowId = GetSheetRowIdWithInsertingNewDayRowIfNecessary(gameReg, games);
                _sheetWriter.InsertEmptyRow(sheetRowId);
                var tableValues = GetNewGameTableValues(gameReg);
                _sheetWriter.FormatNewGameRow(sheetRowId);
                _sheetWriter.WriteNewGameValues(tableValues, sheetRowId);
            }

            return gameReg;
        }

        private long GetSheetRowIdWithInsertingNewDayRowIfNecessary(GameRegistration gameReg, List<GameRegistration> games)
        {
            long sheetRowId = -1;
            bool isInsertingNewDayRowNeeded = false;

            var sameDayGames = games
                .Where(g => g.Date.HasValue && g.Date.Value.Date == gameReg.Date.Value.Date);

            var lastPreviousGame = games
                .Where(g => g.Date.HasValue && g.Date.Value <= gameReg.Date.Value)
                .OrderBy(g => g.Date.Value.Date)
                .LastOrDefault();

            var firstNextGame = games
               .Where(g => g.Date.HasValue && g.Date.Value > gameReg.Date.Value)
               .OrderBy(g => g.Date.Value.Date)
               .FirstOrDefault();

            if (lastPreviousGame == null)
            {
                sheetRowId = 2;
            }
            else
            {
                if (sameDayGames.Count() == 0 || sameDayGames.Contains(lastPreviousGame))
                {
                    sheetRowId = lastPreviousGame.SheetRowId + 1;
                }
                else
                {
                    sheetRowId = firstNextGame.SheetRowId;
                }
            }
            if (sameDayGames.Count() == 0)
            {
                isInsertingNewDayRowNeeded = true;
            }

            if (isInsertingNewDayRowNeeded)
            {
                _sheetWriter.InsertEmptyRow(sheetRowId);
                sheetRowId++;
                _sheetWriter.InsertNewDayRow(sheetRowId, gameReg.Date.Value);
                sheetRowId++;
            }

            return sheetRowId;
        }

        private async Task<int> GetCurrentStepNum(long userId)
        {
            var lastDialog = await _dialogLogic.GetLastActiveDialog(userId);
            var lastCompletedStep = await _dialogLogic.GetLastDialogStep(lastDialog.Id);

            return lastCompletedStep != null ? lastCompletedStep.StepNum : 0;
        }

        private bool ValidateStepValue(string value, int currentStepNum)
        {
            var regex = new Regex(_steps[currentStepNum].Regex);
            return regex.IsMatch(value);
        }

        private bool ValidateStepValueAdditional(string value, int currentStepNum)
        {
            if (currentStepNum == 0)
            {
                var date = ParseHelper.TryParseDate(value);
                if (date.HasValue && date.Value < DateTime.Now.Date)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task SendStepMessage(long chatId, int stepNum)
        {
            var client = _botClient.GetClient();
            var buttonCaptions = _steps[stepNum].ButtonCaptions;
            InlineKeyboardMarkup keyboardMarkup = null;
            if (buttonCaptions != null && buttonCaptions.Count > 0)
            {
                var officialGamedays = _sheetParser.ParseOfficialGamedays();
                keyboardMarkup = KeyboardMarkupHelper.GetInlineKeyboardMarkup(_steps[stepNum].ButtonCaptions, 4, officialGamedays);
            }
            await client.SendTextMessageAsync(new ChatId(chatId), $"{_steps[stepNum].Message}{Environment.NewLine}{Environment.NewLine}" +
                $"<i>" +
                $"/pause — приостановить этот диалог{Environment.NewLine}" +
                $"/cancel — завершить этот диалог" +
                $"</i>",
                Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: keyboardMarkup);
        }

        private void InitStepMessagesAndValidations()
        {
            _steps = new List<DialogStepAdditional>();
            var messages = ResourceHelper.GetAllResourcesEntries(DialogStepsMessages.ResourceManager);
            var regexes = ResourceHelper.GetAllResourcesEntries(DialogStepsRegexes.ResourceManager);
            var buttonCaptions = ResourceHelper.GetAllResourcesEntries(DialogStepsButtons.ResourceManager);

            foreach (var message in messages)
            {
                var f = buttonCaptions.FirstOrDefault(b => b.Key == message.Key);
                var v = f.Value;
                var spl = v?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _steps.Add(new DialogStepAdditional()
                {
                    Key = message.Key,
                    Message = message.Value,
                    Regex = regexes.FirstOrDefault(r => r.Key == message.Key).Value,
                    ButtonCaptions = buttonCaptions
                        .FirstOrDefault(b => b.Key == message.Key)
                        .Value
                        ?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        ?.ToList()
                });
            }
        }

        private List<string> GetNewGameTableValues(GameRegistration gameReg)
        {
            var result = new List<string>();
            var tableColumns = Enum.GetValues(typeof(RegistrationTableColumnsOrder))
                .Cast<RegistrationTableColumnsOrder>()
                .OrderBy(col => (int)col);

            foreach (RegistrationTableColumnsOrder column in tableColumns)
            {
                string value = null;
                switch (column)
                {
                    case RegistrationTableColumnsOrder.Time:
                        value = gameReg.TimeRaw;
                        break;
                    case RegistrationTableColumnsOrder.Place:
                        value = gameReg.Place;
                        break;
                    case RegistrationTableColumnsOrder.Name:
                        value = gameReg.Name;
                        break;
                    case RegistrationTableColumnsOrder.Description:
                        value = gameReg.Description;
                        break;
                    case RegistrationTableColumnsOrder.Link:
                        value = gameReg.Link;
                        break;
                    case RegistrationTableColumnsOrder.Host:
                        value = gameReg.Host.ToString();
                        break;
                    case RegistrationTableColumnsOrder.Duration:
                        value = gameReg.Duration;
                        break;
                    case RegistrationTableColumnsOrder.MaxPlayers:
                        value = gameReg.MaxPlayers.ToString();
                        break;
                    default:
                        break;
                }

                if (value != null)
                {
                    result.Add(value);
                }
            }

            foreach (var player in gameReg.Players)
            {
                var playerStr = player == null
                    ? String.Empty
                    : player.ToString();
                result.Add(playerStr);
            }

            return result;
        }
    }
}
