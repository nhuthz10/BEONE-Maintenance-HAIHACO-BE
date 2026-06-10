using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maintenance.Entities.Responses
{
    public class ResponseApi<T>
    {
        public int ErrCode { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T Data { get; set; }

        public ResponseApi(int errCode, string message, T data = default)
        {
            ErrCode = errCode;
            Message = message;
            Data = data;
        }

        public static ResponseApi<T> Success(T data = default, string message = "Success")
        {
            return new ResponseApi<T>(0, message, data);
        }

        public static ResponseApi<T> Error(ErrorCode errorCode, string message)
        {
            return new ResponseApi<T>((int)errorCode, message);
        }
    }
}
