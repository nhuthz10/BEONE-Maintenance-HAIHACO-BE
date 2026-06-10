using Maintenance.Entities.Auth;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Auth
{
    public interface IAuthRepository
    {
        public Task<OperationResult<ViewLoginModel>> AppSignIn(LoginModel model);
        public Task<OperationResult<ViewLoginModel>> SignIn(LoginAdminModel model);
        public Task<OperationResult<bool>> UnlockoutUser(string id);
        public Task<OperationResult<bool>> LockoutUser(string id);
        public Task<OperationResult<bool>> CheckLockUser(string id);
    }
}
