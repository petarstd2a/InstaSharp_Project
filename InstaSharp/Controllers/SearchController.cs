using InstaSharp.Data.Context;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InstaSharp.Controllers
{
    public class SearchController : Controller
    {
        private readonly InstaDbContext _ctx = new InstaDbContext();

        [HttpGet]
        public async Task<ActionResult> Index(String search)
        {
            if (String.IsNullOrWhiteSpace(search))
                return Redirect("~/");

            var s = search.ToLower();
            ViewBag.Search = search;

            // Fetch profiles that match by user name, email or real name.
            // Work will need to be done to ensure privacy settings are taken into account, 
            // e.g people might not want to be found by their email.
            var profiles = await _ctx.Users.Where(p => p.UserName.ToLower().Contains(s)
                || p.Email.ToLower().Contains(s)
                || p.RealName.ToLower().Contains(s)
            ).ToListAsync();

            return View(profiles);
        }
    }
}
