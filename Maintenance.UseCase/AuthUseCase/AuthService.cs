using Maintenance.Entities.Auth;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Repositories.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.AuthUseCase
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<OperationResult<ViewLoginModel>> AppSignIn(LoginModel model)
        {
            try
            {
                var result = await _authRepository.AppSignIn(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<ViewLoginModel>> SignIn(LoginAdminModel model)
        {
            try
            {
                var result = await _authRepository.SignIn(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> UnlockoutUser(string id)
        {
            try
            {
                var result = await _authRepository.UnlockoutUser(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> LockoutUser(string id)
        {
            try
            {
                var result = await _authRepository.LockoutUser(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> CheckLockUser(string id)
        {
            try
            {
                var result = await _authRepository.CheckLockUser(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> CreateUser(CreateUserModel model)
        {
            try
            {
                var result = await _authRepository.CreateUser(model);
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
