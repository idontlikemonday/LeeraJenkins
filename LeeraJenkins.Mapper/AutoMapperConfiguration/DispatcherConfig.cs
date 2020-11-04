using AutoMapper;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Mapper.AutoMapperConfiguration
{
    public static class DispatcherConfig
    {
        public static void ConfigureDispatcherStatusesMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<DialogDispatcherResult, DispatcherResult>()
                .ConvertUsing(v => Convert(v));
        }

        private static DispatcherResult Convert(DialogDispatcherResult v)
        {
            switch (v)
            {
                case DialogDispatcherResult.NoResult:
                    return DispatcherResult.NoResult;
                case DialogDispatcherResult.Success:
                    return DispatcherResult.Success;
                case DialogDispatcherResult.ValidationError:
                    return DispatcherResult.ValidationError;
                case DialogDispatcherResult.NullUser:
                    return DispatcherResult.NullUser;
                case DialogDispatcherResult.NoActiveDialogs:
                    return DispatcherResult.NoActiveDialogs;
                case DialogDispatcherResult.DateFromPast:
                    return DispatcherResult.DateFromPast;
                case DialogDispatcherResult.Finalized:
                    return DispatcherResult.Finalized;

                default:
                    return DispatcherResult.NoResult;
            }
        }
    }
}
