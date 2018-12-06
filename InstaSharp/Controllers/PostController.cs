using InstaSharp.Data.Context;
using InstaSharp.Data.Models;
using InstaSharp.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InstaSharp.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly InstaDbContext _ctx = new InstaDbContext();
        private readonly IPostService _postService;

        public PostController(IPostService _postService)
        {
            this._postService = _postService;
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var post = await _ctx.Posts.FindAsync(id);
            if (post == null)
                return HttpNotFound();

            if (Request.IsAjaxRequest())
                return PartialView(post);

            return View(post);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post post, HttpPostedFileBase imageFile)
        {
            string uploadDirectory = Server.MapPath(String.Format("~/Images/Uploads/{0}/", User.Identity.Name));
            var postMedia = _postService.SavePostMedia(User.Identity.Name, post, imageFile, uploadDirectory, _ctx);

            if (!string.IsNullOrEmpty(postMedia))
            {
                if (ModelState.IsValid)
                {
                    await _postService.SavePost(User.Identity.Name, postMedia, post, _ctx);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("Image", "Please select an image to share.");
            }

            return View(post);
        }

        //[HttpGet]
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = await _ctx.Posts.FindAsync(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Post post = await _ctx.Posts.FindAsync(id);
        //    _ctx.Posts.Remove(post);
        //    await _ctx.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
