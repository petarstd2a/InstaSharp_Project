using InstaSharp.Data.Context;
using InstaSharp.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InstaSharp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly InstaDbContext _ctx = new InstaDbContext();
        private readonly IPostService _postService;

        public HomeController(IPostService _postService)
        {
            this._postService = _postService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var posts = await _postService.GetTimelinePosts(User.Identity.Name, _ctx);
            return View(posts);
        }
    }
}