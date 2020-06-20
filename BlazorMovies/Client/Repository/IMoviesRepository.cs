using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public interface IMoviesRepository
    {
        Task<int> CreateMovie(Movie m);
        Task<DetailMovieDTO> GetDetailsMovieDTO(int id);
        Task<IndexPageDTO> GetIndexPageDTO();
        Task<MovieUpdateDTO> GetMovieForUpdate(int id);
        Task Update(Movie movie);
        Task DeleteMovie(int id);
        Task<PaginatedResponse<List<Movie>>> GetMoviesFilter(FilterMovieDTO filterMovieDTO);
        Task<List<Movie>> GetMoviesByActor(int personId);
    }
}
