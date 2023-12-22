using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public IUserRepository UserRepository => new UserRepository(_dataContext, _mapper);
        public IMessageRepository MssageRepository => new MessageRepository(_dataContext, _mapper);

        public ILikedRepository LikedRepository => new LikeRepository(_dataContext);

        public IPhotoRepository PhotoRepository => new
        PhotoRepository(_dataContext);

        public async Task<bool> Complete()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public bool HasVhanges()
        {
            return _dataContext.ChangeTracker.HasChanges();
        }
    }
}