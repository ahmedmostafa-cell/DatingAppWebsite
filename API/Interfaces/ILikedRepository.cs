using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikedRepository
    {

        Task<UserLike> GetUserLike(int SourceUserId, int TargetUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDtos>> GetUserLikes(LikesParams likesParams);


    }
}