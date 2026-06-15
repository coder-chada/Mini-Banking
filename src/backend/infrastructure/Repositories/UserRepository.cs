using Infrastructure.PersistenceModels;
using ApplicationService.Users.Contracts;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly MyDBContext _myDBContext;

        public UserRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Func<int>> AddUserAsync(User user,
                                                  CancellationToken cancellationToken = default)
        {
            var userEntity = MapToEntityPersistenceFrom(user);

            await _myDBContext.Users.AddAsync(userEntity,
                                              cancellationToken).ConfigureAwait(false);

            return () => user.Id;
        }

        private static UserEntity MapToEntityPersistenceFrom(User user)
        {
            if (user is null) 
                throw new System.ArgumentNullException(nameof(user));

            return new UserEntity(user.Id,
                                  user.DNI,
                                  user.Nombres,
                                  user.Apellidos,
                                  user.Email);
        }

        public Task<User> GetUserBy(int id,
                                    CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
