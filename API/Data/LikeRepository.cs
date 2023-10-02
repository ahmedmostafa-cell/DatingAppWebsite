using System.Security.AccessControl;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikeRepository : ILikedRepository
    {
        private readonly DataContext _context;
        public LikeRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int SourceUserId, int TargetUserId)
        {
            return await _context.Likes.FindAsync(SourceUserId, TargetUserId);
        }

        public async Task<PagedList<LikeDtos>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDtos
            {
                UserName = user.UserName,
                Id = user.Id,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.calculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain).Url,
                City = user.City

            });

            return await PagedList<LikeDtos>.CreateAync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}