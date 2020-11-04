namespace LeeraJenkins.Model.Notifications
{
    public class NotificationBaseModel
    {
        public string Value { get; set; }
        public string OldValue { get; set; }
        public int RowIndex { get; set; }
        public bool IsPlayerChangedNotification { get; set; }
        public bool IsHostChangedNotification { get; set; }
        public bool IsTimeChangedNotification { get; set; }
        public bool IsExceedPlayer { get; set; }
    }
}
