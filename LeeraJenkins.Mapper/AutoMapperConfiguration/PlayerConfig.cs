using AutoMapper;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class PlayerConfig
    {
        public static void ConfigurePlayerMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Db.GamePlayer, Model.Core.Player>()
                .ForMember(dbm => dbm.Name, map => map.MapFrom(m => m.Player.Name))
                .ForMember(dbm => dbm.TgNickname, map => map.MapFrom(m => m.Player.Name))
            ;

            config.CreateMap<Model.Core.Player, Db.GamePlayer>()
                .ForMember(m => m.Player, map => map.MapFrom(dbm => dbm))
            ;

            config.CreateMap<Model.Core.Player, Db.Player>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(dbm => dbm.TgName, map => map.MapFrom(m => m.TgNickname))
            ;

            config.CreateMap<Db.Player, Model.Core.Player>()
                .ForMember(m => m.Id, map => map.MapFrom(dbm => dbm.Id))
                .ForMember(m => m.Name, map => map.MapFrom(dbm => dbm.Name))
                .ForMember(m => m.TgNickname, map => map.MapFrom(dbm => dbm.TgName))
            ;

            config.CreateMap<User, Model.Core.Player>()
                .ForMember(p => p.Name, map => map.MapFrom(u => u.FirstName))
                .ForMember(p => p.TgNickname, map => map.MapFrom(u => $"@{u.Username}"))
            ;

            config.CreateMap<Chat, Model.Core.Player>()
                .ForMember(p => p.Name, map => map.MapFrom(u => u.FirstName))
                .ForMember(p => p.TgNickname, map => map.MapFrom(u => $"@{u.Username}"))
            ;
        }
    }
}
