using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application
{
    public class APIResponse<T>
    {
    
            public bool Success { get; set; }
            public string Message { get; set; }
            public T? Data { get; set; }

            public static APIResponse<T> SuccessResponse(T data, string message)
                => new APIResponse<T> { Success = true, Message = message, Data = data };

            public static APIResponse<T> FailResponse(string message)
                => new APIResponse<T> { Success = false, Message = message };
     

}
}
