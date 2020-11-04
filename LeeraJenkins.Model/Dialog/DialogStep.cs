using System;

namespace LeeraJenkins.Model.Dialog
{
    public class DialogStep
    {
        public long Id { get; set; }
        public long UserDialogId { get; set; }
        public int StepNum { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }
}
