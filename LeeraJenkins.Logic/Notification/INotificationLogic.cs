using System.Threading.Tasks;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;

namespace LeeraJenkins.Logic.Notification
{
    public interface INotificationLogic
    {
        Task NotifyRegularDaily();
        Task NotifyRegularSeparate();
        Task NotifyUsersWithChecking(NotificationPlayersModel model);
        Task NotifyUsers(GameRegistration game, NotificationBaseModel model = null, NotificationType notificationType = NotificationType.Unknown);
    }
}
