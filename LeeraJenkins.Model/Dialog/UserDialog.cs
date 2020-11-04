using System;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Model.Dialog
{
    public class UserDialog
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DialogType DialogId { get; set; }
        public DateTime Date { get; set; }
        public DialogStatus Status { get; set; }
    }
}
