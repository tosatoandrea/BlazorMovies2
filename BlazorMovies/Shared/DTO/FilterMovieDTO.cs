using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMovies.Shared.DTO
{
    public class FilterMovieDTO
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 5;
        public PaginationDTO Pagination 
        { 
            get { return new PaginationDTO { Page = Page, RecordPerPage = RecordsPerPage }; }
        }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpcomingReleases { get; set; }

    }
}
