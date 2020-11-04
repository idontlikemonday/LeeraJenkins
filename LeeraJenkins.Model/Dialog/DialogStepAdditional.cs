using System.Collections.Generic;

namespace LeeraJenkins.Model.Dialog
{
    public class DialogStepAdditional
    {
        public string Key { get; set; }
        public string Message { get; set; }
        public string Regex { get; set; }
        public List<string> ButtonCaptions { get; set; }
    }
}
