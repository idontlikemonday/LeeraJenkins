using AutoMapper;
using LeeraJenkins.Model.Logger;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class LogConfig
    {
        public static void ConfigureLogMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Log, Db.Log>()
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
                .ForMember(dbm => dbm.Message, map => map.MapFrom(m => m.Message))
                .ForMember(dbm => dbm.StackTrace, map => map.MapFrom(m => m.StackTrace))
            ;

            config.CreateMap<Db.Log, Log>()
                .ForMember(m => m.Date, map => map.MapFrom(dbm => dbm.Date))
                .ForMember(m => m.Message, map => map.MapFrom(dbm => dbm.Message))
                .ForMember(m => m.StackTrace, map => map.MapFrom(dbm => dbm.StackTrace))
            ;
        }

        public static void ConfigureUserActionLogMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<UserActionLog, Db.UserAction>()
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
                .ForMember(dbm => dbm.Action, map => map.MapFrom(m => m.Action))
                .ForMember(dbm => dbm.Username, map => map.MapFrom(m => m.Username))
            ;

            config.CreateMap<Db.UserAction, UserActionLog>()
                .ForMember(m => m.Date, map => map.MapFrom(dbm => dbm.Date))
                .ForMember(m => m.Action, map => map.MapFrom(dbm => dbm.Action))
                .ForMember(m => m.Username, map => map.MapFrom(dbm => dbm.Username))
            ;
        }
    }
}
