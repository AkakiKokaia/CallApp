using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.DTOs
{
    public class PagedResponseModel
    {
        public int page { get; set; } = 0;
        public int count { get; set; } = 10;
    }
}
