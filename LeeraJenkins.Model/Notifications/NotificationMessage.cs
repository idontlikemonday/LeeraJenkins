using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Model.Notifications
{
    public class NotificationChangesModel
    {
        public string RecepientTgName { get; set; }
        public string GameName { get; set; }
        public NotificationType NotificationType { get; set; }
        public string PrevValue { get; set; }
        public string NewValue { get; set; }
    }
}
