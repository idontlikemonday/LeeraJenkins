using AutoMapper;
using System;
using LeeraJenkins.DbRepository.Dialog;
using LeeraJenkins.DbRepository.Game;
using LeeraJenkins.DbRepository.Logger;
using LeeraJenkins.DbRepository.Player;
using LeeraJenkins.DbRepository.User;
using LeeraJenkins.Logic.Bot;
using LeeraJenkins.Logic.Calendar;
using LeeraJenkins.Logic.Dialog;
using LeeraJenkins.Logic.Dispatchers;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.NewGame;
using LeeraJenkins.Logic.Notification;
using LeeraJenkins.Logic.Player;
using LeeraJenkins.Logic.Registration;
using LeeraJenkins.Logic.Sheet;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Mapper.AutoMapperConfiguration;
using Unity;
using LeeraJenkins.Logic.Cache;

namespace LeeraJenkins.Configuration
{
    public static class UnityConfiguration
    {
        private static Lazy<IUnityContainer> container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        public static IUnityContainer Container => container.Value;

        public static void RegisterTypes(IUnityContainer container)
        {
            RegisterBot(container);
            RegisterLogic(container);
            RegisterRepositories(container);
            RegisterParsers(container);
            RegisterDispatchers(container);

            container.RegisterAutoMapper();
        }

        private static void RegisterBot(IUnityContainer container)
        {
            container.RegisterType<IBotClient, BotClient>();
        }

        private static void RegisterLogic(IUnityContainer container)
        {
            container.RegisterType<IGameLogic, GameLogic>();
            container.RegisterType<IPlayerLogic, PlayerLogic>();
            container.RegisterType<INotificationLogic, NotificationLogic>();
            container.RegisterType<IRegistrationLogic, RegistrationLogic>();
            container.RegisterType<INewMemberNotificationLogic, NewMemberNotificationLogic>();
            container.RegisterType<IPushNotificationLogic, PushNotificationLogic>();
            container.RegisterType<ICacheLogic, CacheLogic>();
            container.RegisterType<ICalendarLogic, CalendarLogic>();
            container.RegisterType<IDialogLogic, DialogLogic>();
            container.RegisterType<IPauseDialogLogic, PauseDialogLogic>();
            container.RegisterType<INewGameLogic, NewGameLogic>();
            container.RegisterType<INotificationMessageGenerator, NotificationMessageGenerator>();
            container.RegisterType<ILogger, Logger>();
            container.RegisterType<IUserActionLogger, UserActionLogger>();
            container.RegisterType<IUserLogic, UserLogic>();
        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            container.RegisterType<IGameRepository, GameRepository>();
            container.RegisterType<IPlayerRepository, PlayerRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ILoggerRepository, LoggerRepository>();
            container.RegisterType<IUserActionLoggerRepository, UserActionLoggerRepository>();
            container.RegisterType<IDialogRepository, DialogRepository>();
        }

        private static void RegisterParsers(IUnityContainer container)
        {
            container.RegisterType<ISheetService, SheetService>();
            container.RegisterType<ISheetParser, SheetParser>();
            container.RegisterType<ISheetWriter, SheetWriter>();
        }

        private static void RegisterDispatchers(IUnityContainer container)
        {
            container.RegisterType<ICallbackDispatcher, CallbackDispatcher>();
            container.RegisterType<IDialogMessageDispatcher, DialogMessageDispatcher>();
        }

        private static IUnityContainer RegisterAutoMapper(this IUnityContainer container)
        {
            var mappedConfiguration = new MapperConfiguration(config =>
            {
                config.ConfigureGameMapper();
                config.ConfigurePlayerMapper();
                config.ConfigureUserMapper();
                config.ConfigureLogMapper();
                config.ConfigureUserActionLogMapper();
                config.ConfigureDialogMapper();
                config.ConfigureDispatcherStatusesMapper();
            });

            container.RegisterInstance(typeof(IMapper), mappedConfiguration.CreateMapper());

            return container;
        }
    }
}
