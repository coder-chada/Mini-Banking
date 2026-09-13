using ApplicationService.Users.Contracts;
using DomainLogic.Entities;
using Infrastructure.PersistenceModels;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly MyDBContext _myDBContext;

        public UserRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Func<int>> AddAsync(
            User user,
            CancellationToken cancellationToken = default
        )
        {
            var userEntity = MapToEntityPersistenceFrom(user);

            await _myDBContext
                .Users
                .AddAsync(userEntity, cancellationToken)
                .ConfigureAwait(false);

            return () => userEntity.ID;
        }

        private static UserEntity MapToEntityPersistenceFrom(User user)
        {
            if (user is null)
                throw new ArgumentNullException("Entity USER can not be null");

            return new UserEntity(user.ID, user.DNI, user.Nombres, user.Apellidos, user.Email);
        }

        public async Task<User> GetBy(int id, CancellationToken cancellationToken = default)
        {
            var userEntity = await _myDBContext
                .Users
                .FindAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (userEntity is null)
                throw new ArgumentException($"ID {id} does not exists");

            var user = MapToEntityFrom(userEntity);

            return user;
        }

        private static User MapToEntityFrom(UserEntity userEntity)
        {
            if (userEntity is null)
                throw new ArgumentNullException($"Entity USER_ENTITY can not be null");

            return new User(
                userEntity.ID,
                userEntity.DNI,
                userEntity.Nombres,
                userEntity.Apellidos,
                userEntity.Correo
            );
        }
    }
}
