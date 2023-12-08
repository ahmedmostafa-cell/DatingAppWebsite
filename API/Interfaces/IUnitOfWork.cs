using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }

        IMessageRepository MssageRepository { get; }

        ILikedRepository LikedRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        Task<bool> Complete();

        bool HasVhanges();
    }
}