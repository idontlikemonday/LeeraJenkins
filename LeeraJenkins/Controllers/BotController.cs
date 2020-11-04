using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using LeeraJenkins.Code;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Dispatchers;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.Notification;

namespace LeeraJenkins.Controllers
{
    [RoutePrefix("api/bot")]
    public class BotController : ApiController
    {
        private IBot _bot;
        private IBotClient _botClient;
        private ICallbackDispatcher _dispatcher;
        private IDialogMessageDispatcher _dialogDispatcher;
        private INewMemberNotificationLogic _nmNotification;
        private INewGameLogic _newGameLogic;
        private ILogger _logger;
        private IUserActionLogger _userActionLogger;

        public BotController(IBot bot, IBotClient botClient, ICallbackDispatcher dispatcher, IDialogMessageDispatcher dialogDispatcher,
            INewMemberNotificationLogic nmNotification, ILogger logger, IUserActionLogger userActionLogger, INewGameLogic newGameLogic)
        {
            _bot = bot;
            _botClient = botClient;
            _dispatcher = dispatcher;
            _dialogDispatcher = dialogDispatcher;
            _nmNotification = nmNotification;
            _logger = logger;
            _userActionLogger = userActionLogger;
            _newGameLogic = newGameLogic;
        }

        [HttpGet]
        [Route("get")]
        public string Get()
        {
            return "Get";
        }

        [HttpPost]
        [Route("update")]
        public async Task Post([FromBody]Update update)
        {
            try
            {
                if (update == null)
                {
                    return;
                }

                _bot.InitializeCommands();
                var client = _botClient.GetClient();

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    bool succeedExecution = false;

                    if (!succeedExecution) succeedExecution = await CheckNewMemberJoining(client, message);
                    if (!succeedExecution) succeedExecution = await CheckCommandExecution(client, message);
                    if (!succeedExecution) succeedExecution = await CheckSpecialCommandExecution(client, message);
                    if (!succeedExecution) await CheckAnswerAsDialogAnswer(client, message);
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    var callbackQuery = update.CallbackQuery;
                    var result = await _dispatcher.ExecuteDispatch(callbackQuery);
                    var resultDescription = result.GetDescription();

                    if (!String.IsNullOrEmpty(resultDescription))
                    {
                        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, resultDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при выполнении команды бота", ex);
            }
        }

        private async Task<bool> CheckNewMemberJoining(Telegram.Bot.TelegramBotClient client, Message message)
        {
            if (message.NewChatMembers != null && message.NewChatMembers.Count() > 0)
            {
                var member = message.NewChatMembers.FirstOrDefault();
                if (!member.IsBot)
                {
                    await _nmNotification.SayHi(client, member, message.Chat.Id);
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> CheckCommandExecution(Telegram.Bot.TelegramBotClient client, Message message)
        {
            bool result = false;
            foreach (var command in _bot.Commands)
            {
                if (command.Name.Equals(message.Text)
                    || command.Aliases.Contains(message.Text))
                {
                    if (message.Chat.Type != ChatType.Private)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                            "Я не отвечаю в публичных группах. Пожалуйста, напиши мне лично");
                    }
                    else
                    {
                        await command.Execute(message, client);
                        _userActionLogger.AddUserAction($"@{message.From.Username}", message.Text);
                    }

                    result = true;
                }
            }

            return result;
        }

        private async Task<bool> CheckSpecialCommandExecution(Telegram.Bot.TelegramBotClient client, Message message)
        {
            bool result = false;
            foreach (var command in _bot.SpecialCommands)
            {
                if (command.Name.Equals(message.Text)
                    || command.Aliases.Contains(message.Text)
                    || message.Text.StartsWith(command.Name))
                {
                    if (message.Chat.Type != ChatType.Private)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                            "Я не отвечаю в публичных группах. Пожалуйста, напиши мне лично");
                    }
                    else
                    {
                        await command.Execute(message, client);
                        _userActionLogger.AddUserAction($"@{message.From.Username}", message.Text);
                    }

                    result = true;
                }
            }

            return result;
        }

        private async Task CheckAnswerAsDialogAnswer(Telegram.Bot.TelegramBotClient client, Message message)
        {
            var result = await _dialogDispatcher.PerformDialogStep(message.Chat, message.Text);
            var resultDescription = result.GetDescription();

            if (!String.IsNullOrEmpty(resultDescription))
            {
                await client.SendTextMessageAsync(message.Chat.Id, resultDescription);
            }
        }
    }
}
