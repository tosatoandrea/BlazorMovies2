using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public class MoviesRepository : IMoviesRepository
    {
        private string url = "api/movies";
        private readonly IHttpService httpService;

        public MoviesRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<IndexPageDTO> GetIndexPageDTO()
        {
            return await httpService.GetHelper<IndexPageDTO>(url);
        }

        public async Task<DetailMovieDTO> GetDetailsMovieDTO(int id)
        {
            return await httpService.GetHelper<DetailMovieDTO>($"{url}/{id}");
        }

        public async Task<MovieUpdateDTO> GetMovieForUpdate(int id)
        {
            return await httpService.GetHelper<MovieUpdateDTO>($"{url}/update/{id}");
        }
        

        public async Task<int> CreateMovie(Movie m)
        {
            var response = await httpService.Post<Movie, int>(url, m);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task Update(Movie movie)
        {
            var response = await httpService.Put(url, movie);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task DeleteMovie(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<List<Movie>> GetMoviesByActor(int personId)
        {
            var response = await httpService.Get<List<Movie>>($"{url}/actor/{personId}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }
        

        public async Task<PaginatedResponse<List<Movie>>> GetMoviesFilter(FilterMovieDTO filterMovieDTO)
        {
            var responseHTTP = await httpService.Post<FilterMovieDTO, List<Movie>>($"{url}/filter", filterMovieDTO);
            var totalAmountOfPages = int.Parse(responseHTTP.HttpResponseMessage.Headers.GetValues("totalAmountOfPages").FirstOrDefault());
            var paginatedResponse = new PaginatedResponse<List<Movie>>
            {
                Response = responseHTTP.Response,
                TotalAmountOfPages = totalAmountOfPages
            };
            return paginatedResponse;
        }
    }
}
