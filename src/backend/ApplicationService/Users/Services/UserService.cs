
using ApplicationService.Common.Contracts;
using ApplicationService.Common.Exceptions;
using ApplicationService.Users.Contracts;
using ApplicationService.Users.DTOs;
using DomainLogic.Entities;

namespace ApplicationService.Users.Services
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserDTO,
                                                              CancellationToken cancellationToken = default)
        {
            if (createUserDTO is null)
                throw new ApplicationServiceException(ApplicationServiceErrorCode.MissingOrInvalidData,
                                                      "User can not be null");

            var user = new User(createUserDTO.DNI,
                                createUserDTO.Nombres,
                                createUserDTO.Apellidos,
                                createUserDTO.Email);
            
            var newUserID = await _unitOfWork
                .UserRepository
                .AddUserAsync(user, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            var response = new CreateUserResponse(newUserID());

            return response;
        }
    }
}
