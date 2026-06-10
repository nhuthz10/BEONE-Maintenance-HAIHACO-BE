using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Responses
{
    public class OperationResult<T>
    {
        public bool IsSuccess => ErrorCode == ErrorCode.None;
        public ErrorCode ErrorCode { get; set; } = ErrorCode.None;
        public string Message { get; set; } = "Success";
        public T Data { get; set; }

        public static OperationResult<T> Success(T data, string message = "Success")
            => new OperationResult<T> { Data = data, Message = message };

        public static OperationResult<T> Fail(ErrorCode code, string message)
            => new OperationResult<T> { ErrorCode = code, Message = message };
    }
}
