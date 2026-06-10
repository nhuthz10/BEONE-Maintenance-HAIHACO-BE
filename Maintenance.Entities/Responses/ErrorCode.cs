using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Responses
{
    public enum ErrorCode
    {
        None = 0,
        InternalServerError = 500,
        ValidationError = 400,
        Unauthorized = 401,
        ValidationFailed = 403,
        NotFound = 404,
        InvalidVersion = 426,
        ErrorSap = 420,

        //Account
        InvalidUsernamePassword = 410,
        AccountLocked = 411,
        DeviceDontMatch = 413,
        EmailAlreadyExists = 414,
        UserNameAlreadyExists = 415,
        FailedCreateAccount = 416,
        FailedAssignRole = 417,
        TokenExpired = 418,
        AccountAlreadyOnline = 419,
    }
}
