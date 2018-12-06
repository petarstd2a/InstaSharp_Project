using InstaSharp.Data.Context;
using InstaSharp.Services;
using InstaSharp.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InstaSharp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly InstaDbContext _ctx = new InstaDbContext();
        private readonly IUserService _userService;
        private readonly IFollowService _followService;

        public ProfileController(IUserService _userService,
                                IFollowService _followService)
        {
            this._userService = _userService;
            this._followService = _followService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Details(string id)
        {
            var userId = id ?? string.Empty;

            // Don't try load a profile called /profile/
            if (id.ToLower() == "profile" && Request.IsAuthenticated && User.Identity.Name != "profile")
                userId = string.Empty;

            // No user ID provided, show user's profile if currently logged in
            if (string.IsNullOrEmpty(userId) && Request.IsAuthenticated)
            {
                var loggedInUser = await _userService.GetByUsername(User.Identity.Name, _ctx);
                userId = loggedInUser.Id;
            }

            // Find profile details
            var user = await _userService.GetByUsernameOrId(userId, _ctx);
            if (user == null) return Redirect("~/");

            var model = new ProfileViewModel
            {
                User = user,
                PostCount = user.Posts != null && user.Posts.Any() ? user.Posts.Count : 0,
                FollowerCount = await _followService.FollowerCount(user.UserName, _ctx),
                FollowingCount = await _followService.FollowingCount(user.UserName, _ctx),
                Following = false,
                OwnProfile = false
            };

            if (Request.IsAuthenticated)
            {
                model.Following = await _followService.IsFollowing(User.Identity.Name, user.UserName, _ctx);
                model.OwnProfile = user.UserName == User.Identity.Name;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Following(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var following = await _followService.GetFollowing(id, _ctx);
            ViewBag.Message = String.Format("Following", id);

            return PartialView("_UserList", following);
        }

        [HttpGet]
        public async Task<ActionResult> Followers(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var followers = await _followService.GetFollowers(id, _ctx);
            ViewBag.Message = String.Format("Followers", id);

            return PartialView("_UserList", followers);
        }
    }
}