using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMovies.Shared.DTO
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int RecordPerPage { get; set; } = 10;
    }
}
