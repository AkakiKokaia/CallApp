using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int Code { get; set; }
        public ApiException() : base() { }

        public ApiException(string message, int code) : base(message)
        {
            Code = code;
        }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
