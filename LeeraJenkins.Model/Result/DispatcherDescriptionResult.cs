using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Model.Result
{
    public class DispatcherDescriptionResult
    {
        public DispatcherResult Code { get; set; }
        public string Description { get; set; }

        public DispatcherDescriptionResult(DispatcherResult code)
        {
            Code = code;
            Description = code.GetDescription();
        }
    }
}
