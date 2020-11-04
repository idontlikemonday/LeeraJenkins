using System.Threading.Tasks;

namespace LeeraJenkins.Logic.Notification
{
    public interface IPushNotificationLogic
    {
        Task<int> SendMessageToAll(string message);
    }
}
