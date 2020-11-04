using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using LeeraJenkins.Db;
using LeeraJenkins.Logic.Helpers;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class GameConfig
    {
        public static void ConfigureGameMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Model.Core.GameRegistration, Db.GameRegistration>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.Guid, map => map.MapFrom(m => m.Guid))
                .ForMember(dbm => dbm.SheetRowId, map => map.MapFrom(m => m.SheetRowId))
                .ForMember(dbm => dbm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(dbm => dbm.Status, map => map.MapFrom(m => m.Status))
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
                .ForMember(dbm => dbm.Place, map => map.MapFrom(m => m.Place))
                .ForMember(dbm => dbm.Description, map => map.MapFrom(m => m.Description))
                .ForMember(dbm => dbm.Duration, map => map.MapFrom(m => m.Duration))
                .ForMember(dbm => dbm.Link, map => map.MapFrom(m => m.Link))
                .ForMember(dbm => dbm.MaxPlayers, map => map.MapFrom(m => m.MaxPlayers))
                .ForMember(dbm => dbm.DateRaw, map => map.MapFrom(m => m.DateRaw))
                .ForMember(dbm => dbm.TimeRaw, map => map.MapFrom(m => m.TimeRaw))
                .ForMember(dbm => dbm.Host, map => map.MapFrom(m => m.Host.ToString()))
                .ForMember(dbm => dbm.GamePlayer, map => map.MapFrom(
                    m => m.Players.Select(p =>
                        new GamePlayer()
                        {
                            Player = new Player()
                            {
                                Name = p.Name,
                                TgName = p.TgNickname
                            }
                        }
                )))
                .ForMember(dbm => dbm.Created, map => map.MapFrom(m => DateTime.Now))
            ;

            config.CreateMap<Db.GameRegistration, Model.Core.GameRegistration>()
                .ForMember(m => m.Id, map => map.MapFrom(dbm => dbm.Id))
                .ForMember(m => m.Guid, map => map.MapFrom(dbm => dbm.Guid))
                .ForMember(m => m.SheetRowId, map => map.MapFrom(dbm => dbm.SheetRowId))
                .ForMember(m => m.Name, map => map.MapFrom(dbm => dbm.Name))
                .ForMember(m => m.Status, map => map.MapFrom(dbm => dbm.Status))
                .ForMember(m => m.Date, map => map.MapFrom(dbm => dbm.Date))
                .ForMember(m => m.Place, map => map.MapFrom(dbm => dbm.Place))
                .ForMember(m => m.Description, map => map.MapFrom(dbm => dbm.Description))
                .ForMember(m => m.Duration, map => map.MapFrom(dbm => dbm.Duration))
                .ForMember(m => m.Link, map => map.MapFrom(dbm => dbm.Link))
                .ForMember(m => m.MaxPlayers, map => map.MapFrom(dbm => dbm.MaxPlayers))
                .ForMember(m => m.DateRaw, map => map.MapFrom(dbm => dbm.DateRaw))
                .ForMember(m => m.TimeRaw, map => map.MapFrom(dbm => dbm.TimeRaw))
                .ForMember(m => m.Host, map => map.MapFrom(dbm => ParseHelper.FromString(dbm.Host)))
                .ForMember(m => m.Players, map => map.MapFrom(dbm => dbm.GamePlayer.Select(gp => gp.Player)))
                .AfterMap((dbm, m) => m.MarkExcessPlayers())
            ;
        }
    }
}
