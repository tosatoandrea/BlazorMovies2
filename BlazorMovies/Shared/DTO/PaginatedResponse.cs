using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMovies.Shared.DTO
{
    public class PaginatedResponse<T>
    {
        public T Response { get; set; }
        public int TotalAmountOfPages { get; set; }
    }
}
