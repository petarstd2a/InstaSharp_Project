using InstaSharp.Data.Context;
using InstaSharp.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InstaSharp.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly InstaDbContext _ctx = new InstaDbContext();
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService _notificationService)
        {
            this._notificationService = _notificationService;
        }

        public int Count()
        {
            var notifications = _notificationService.GetNotifications(User.Identity.Name, _ctx, includeViewed: false);
            return notifications.Result.Count;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var notifications = await _notificationService.GetNotifications(User.Identity.Name, _ctx);

            await _notificationService.MarkNotificationsViewed(User.Identity.Name, _ctx);

            return View(notifications);
        }
    }
}