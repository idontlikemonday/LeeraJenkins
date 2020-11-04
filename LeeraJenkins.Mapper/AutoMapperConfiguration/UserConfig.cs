using AutoMapper;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class UserConfig
    {
        public static void ConfigureUserMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Db.User, Model.Core.User>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.ChatId, map => map.MapFrom(m => m.ChatId))
                .ForMember(dbm => dbm.TelegramName, map => map.MapFrom(m => m.TelegramName))
            ;

            config.CreateMap<Model.Core.User, Db.User>()
                .ForMember(m => m.Id, map => map.MapFrom(dbm => dbm.Id))
                .ForMember(m => m.ChatId, map => map.MapFrom(dbm => dbm.ChatId))
                .ForMember(m => m.TelegramName, map => map.MapFrom(dbm => dbm.TelegramName))
            ;
        }
    }
}
