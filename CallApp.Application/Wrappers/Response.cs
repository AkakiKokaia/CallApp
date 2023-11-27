using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T? data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string? Error { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public T? Data { get; set; } = default;
    }
}
