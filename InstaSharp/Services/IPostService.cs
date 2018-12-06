using InstaSharp.Data.Context;
using InstaSharp.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace InstaSharp.Services
{
    public class PostService : IPostService
    {
        private readonly IFollowService _followService;
        private readonly IUserService _userService;

        public PostService(IFollowService _followService,
                           IUserService _userService)
        {
            this._followService = _followService;
            this._userService = _userService;
        }

        /// <summary>
        /// Generate a date ordered time line of posts from post by users the user is following
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetTimelinePosts(string userName, InstaDbContext _ctx)
        {
            // Get list of users being followed, we will display posts by them
            var following = await _followService.GetFollowing(userName, _ctx);

            // Remove any posts which aren't made by a user being followed
            var posts = await _ctx.Posts.OrderByDescending(p => p.Timestamp).ToListAsync();
            posts.RemoveAll(p => !following.Contains(p.User) && p.User.UserName != userName);

            return posts;
        }

        /// <summary>
        /// Save the post media to the server
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="post"></param>
        /// <param name="imageFile"></param>
        /// <param name="uploadDirectory"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public string SavePostMedia(string userName, Post post, HttpPostedFileBase imageFile, string uploadDirectory, InstaDbContext _ctx)
        {
            if (imageFile == null || imageFile.ContentLength <= 0) return string.Empty;

            // Save the image to server
            var fileName = Path.GetRandomFileName().Replace(".", "") + ".png";
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);
            var path = Path.Combine(uploadDirectory, fileName);
            imageFile.SaveAs(path);

            return fileName;
        }

        /// <summary>
        /// Save the new post in the database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="postMedia"></param>
        /// <param name="post"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<int> SavePost(string userName, string postMedia, Post post, InstaDbContext _ctx)
        {
            post.Image = postMedia;
            post.Timestamp = DateTime.Now;
            post.User = await _userService.GetByUsername(userName, _ctx);
            _ctx.Posts.Add(post);
            return await _ctx.SaveChangesAsync();
        }
    }

    public interface IPostService
    {
        Task<List<Post>> GetTimelinePosts(string userName, InstaDbContext _ctx);
        string SavePostMedia(string userName, Post post, HttpPostedFileBase imageFile, string uploadDirectory, InstaDbContext _ctx);
        Task<int> SavePost(string userName, string postMedia, Post post, InstaDbContext _ctx);
    }
}