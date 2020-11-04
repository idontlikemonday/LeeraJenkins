using System;
using System.Threading.Tasks;
using System.Web.Http;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.Notification;
using LeeraJenkins.Model.Notifications;

namespace LeeraJenkins.Controllers
{
    [RoutePrefix("api/notify")]
    public class NotificationController : ApiController
    {
        private INotificationLogic _notificationLogic;
        private ILogger _logger;

        public NotificationController(INotificationLogic notificationLogic, ILogger logger)
        {
            _notificationLogic = notificationLogic;
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public string Get()
        {
            return "Get";
        }

        [HttpPost]
        [Route("players")]
        public async Task NotifyPlayers(NotificationPlayersModel model)
        {
            try
            {
                await _notificationLogic.NotifyUsersWithChecking(model);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка уведомления игроков", ex);
            }
        }

        [HttpPost]
        [Route("regular")]
        public async Task NotifyRegular()
        {
            try
            {
                await _notificationLogic.NotifyRegularDaily();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка регулярных уведомлениий", ex);
            }
        }
    }
}
