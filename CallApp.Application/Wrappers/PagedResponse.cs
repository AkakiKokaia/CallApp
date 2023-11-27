using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, string message = null)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = message;
            this.Succeeded = true;
            this.Error = null;
        }

        public PagedResponse(T? data, int pageNumber, int pageSize, int totalCount, string message = null)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.Data = data;
            this.Message = message;
            this.Succeeded = true;
            this.Error = null;
        }
    }
}
