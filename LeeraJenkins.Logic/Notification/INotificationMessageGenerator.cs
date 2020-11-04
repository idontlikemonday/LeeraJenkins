using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;

namespace LeeraJenkins.Logic.Notification
{
    public interface INotificationMessageGenerator
    {
        string GenerateRecepientNotificationMessage(NotificationBaseModel model, GameRegistration game,
            NotificationType notificationType, string recepientTgName);
    }
}
