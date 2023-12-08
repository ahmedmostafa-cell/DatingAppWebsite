using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<PagedList<MemberDtos>> GetMembersAsync(UserParams userParams);
        Task<MemberDtos> GetMemberByUserNameAsync(string userName);

        Task<string> GetUserGender(string userName);


        Task<MemberDtos> GetMemberAsync(string username, bool isCurrentUser);


        Task<AppUser> GetUserByPhotoId(int photoId);

    }
}