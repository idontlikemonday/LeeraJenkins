using AutoMapper;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class DialogConfig
    {
        public static void ConfigureDialogMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Model.Dialog.UserDialog, Db.UserDialog>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.UserId, map => map.MapFrom(m => m.UserId))
                .ForMember(dbm => dbm.DialogId, map => map.MapFrom(m => (int)m.DialogId))
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
                .ForMember(dbm => dbm.Status, map => map.MapFrom(m => (int)m.Status))
            ;
            config.CreateMap<Db.UserDialog, Model.Dialog.UserDialog>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.UserId, map => map.MapFrom(m => m.UserId))
                .ForMember(dbm => dbm.DialogId, map => map.MapFrom(m => (DialogType)m.DialogId))
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
                .ForMember(dbm => dbm.Status, map => map.MapFrom(m => (DialogStatus)m.Status))
            ;

            config.CreateMap<Model.Dialog.DialogStep, Db.DialogStep>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.UserDialogId, map => map.MapFrom(m => m.UserDialogId))
                .ForMember(dbm => dbm.StepNum, map => map.MapFrom(m => m.StepNum))
                .ForMember(dbm => dbm.Value, map => map.MapFrom(m => m.Value))
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
            ;

            config.CreateMap<Db.DialogStep, Model.Dialog.DialogStep>()
                .ForMember(dbm => dbm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(dbm => dbm.UserDialogId, map => map.MapFrom(m => m.UserDialogId))
                .ForMember(dbm => dbm.StepNum, map => map.MapFrom(m => m.StepNum))
                .ForMember(dbm => dbm.Value, map => map.MapFrom(m => m.Value))
                .ForMember(dbm => dbm.Date, map => map.MapFrom(m => m.Date))
            ;
        }
    }
}
