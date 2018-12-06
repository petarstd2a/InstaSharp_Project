using InstaSharp.Data.Context;
using InstaSharp.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace InstaSharp.Services
{
    public class FollowService : IFollowService
    {
        private readonly IUserService _userService;

        public FollowService(IUserService _userService)
        {
            this._userService = _userService;
        }

        /// <summary>
        /// Allow a user to follow another user so they see their posts in their time line.
        /// </summary>
        /// <param name="followerName"></param>
        /// <param name="followedName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<bool> Follow(string followerName, string followedName, InstaDbContext _ctx)
        {
            // Check if user is already following
            dynamic following = await IsFollowing(followedName, followedName, _ctx);
            if (following) return true;

            // Can't follow yourself
            if (followerName == followedName) return false;

            following = new Following
            {
                Accepted = true,
                Timestamp = DateTime.Now,
                UserFollowed = await _userService.GetByUsername(followedName, _ctx),
                UserFollowing = await _userService.GetByUsername(followerName, _ctx)
            };

            _ctx.Following.Add(following);
            await _ctx.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Allow a user to un-follow somebody they were previously following.
        /// </summary>
        /// <param name="followerName"></param>
        /// <param name="followedName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<bool> Unfollow(string followerName, string followedName, InstaDbContext _ctx)
        {
            dynamic following = await IsFollowing(followerName, followedName, _ctx);
            if (!following) return false;

            _ctx.Following.RemoveRange(await _ctx.Following.Where(f =>
                f.UserFollowing.UserName == followerName && f.UserFollowed.UserName == followedName
            ).ToListAsync());

            await _ctx.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Check if a user is following another user.
        /// </summary>
        /// <param name="followerName"></param>
        /// <param name="followedName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<bool> IsFollowing(string followerName, string followedName, InstaDbContext _ctx)
        {
            var following = await _ctx.Following.FirstOrDefaultAsync(f =>
                f.UserFollowing.UserName == followerName && f.UserFollowed.UserName == followedName
            );
            return following != null;
        }

        /// <summary>
        /// Get the total number of users following the provided user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<int> FollowerCount(string userName, InstaDbContext _ctx)
        {
            return await _ctx.Following.Where(f => f.UserFollowed.UserName == userName).CountAsync();
        }

        /// <summary>
        /// Get the total number of users the provided user is following.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<int> FollowingCount(string userName, InstaDbContext _ctx)
        {
            return await _ctx.Following.Where(f => f.UserFollowing.UserName == userName).CountAsync();
        }

        /// <summary>
        /// Get a list of all the users the provided user is being followed by.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="_ctx"></param>
        /// <returns></returns>
        public async Task<List<ApplicationUser>> GetFollowers(string userName, InstaDbContext _ctx)
        {
            return await _ctx.Following
                .Where(f => f.UserFollowed.UserName == userName)
                .Select(f => f.UserFollowing)
                .ToListAsync();
        }

         //<summary>
         //Get a list of all the users the user provided is following
         //</summary>
         //<param name = "userName" ></ param >
         //< param name="_ctx"></param>
         //<returns></returns>
        public async Task<List<ApplicationUser>> GetFollowing(string userName, InstaDbContext _ctx)
        {
            return await _ctx.Following
                .Where(f => f.UserFollowing.UserName == userName)
                .Select(f => f.UserFollowed)
                .ToListAsync();
        }
    }

    public interface IFollowService
    {
        Task<bool> Follow(string followerName, string followedName, InstaDbContext _ctx);
        Task<bool> Unfollow(string followerName, string followedName, InstaDbContext _ctx);
        Task<bool> IsFollowing(string followerName, string followedName, InstaDbContext _ctx);
        Task<int> FollowerCount(string userName, InstaDbContext _ctx);
        Task<int> FollowingCount(string userName, InstaDbContext _ctx);
        Task<List<ApplicationUser>> GetFollowers(string userName, InstaDbContext _ctx);
        Task<List<ApplicationUser>> GetFollowing(string userName, InstaDbContext _ctx);
    }
}