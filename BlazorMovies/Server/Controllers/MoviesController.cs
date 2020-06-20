using AutoMapper;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly string containerName = "movies";

        public MoviesController(ApplicationDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var limit = 6;

            var moviesInThaters = await context.Movies
                .Where(x => x.InTheaters == true)
                .Take(limit)
                .OrderBy(x => x.ReleaseDate)
                .ToListAsync();

            var today = DateTime.Today;

            var upcomingReleases = await context.Movies
                .Where(x => x.InTheaters == false)
                .Take(limit)
                .OrderBy(x => x.ReleaseDate)
                .ToListAsync();

            var response = new IndexPageDTO
            {
                InTheaters = moviesInThaters,
                UpcomingReleases = upcomingReleases
            };

            return Ok(response);
        }

        [HttpGet("actor/{actorId}")]
        public async Task<IActionResult> GetByActor(int actorId)
        {
            var movies = await context.Movies
                .Where(x => x.MoviesActors.Select(x => x.PersonId).Contains(actorId))
                .OrderBy(x => x.ReleaseDate)
                .ToListAsync();

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetailMovieDTO>> Get(int id)
        {
            var movie = await context.Movies.Where(x => x.Id == id)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync();

            if (movie == null) { return NotFound(); }

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            var model = new DetailMovieDTO();
            model.Movie = movie;
            model.Genres = movie.MoviesGenres.Select(x => x.Genre).ToList();
            model.Actors = movie.MoviesActors.Select(x =>
                new Person
                {
                    Name = x.Person.Name,
                    Picture = x.Person.Picture,
                    Character = x.Character,
                    Id = x.PersonId

                }).ToList();

            return model;
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter(FilterMovieDTO filterMovieDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMovieDTO.Title))
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMovieDTO.Title));

            if (filterMovieDTO.InTheaters)
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);

            if (filterMovieDTO.UpcomingReleases)
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > DateTime.Today);

            if (filterMovieDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(filterMovieDTO.GenreId));
            }

            await HttpContext.InsertPaginationParametersInResponse<Movie>(moviesQueryable, 
                filterMovieDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMovieDTO.Pagination).ToListAsync();

            return Ok(movies);

        }

        [HttpGet("update/{id}")]
        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if (movieActionResult.Result is NotFoundResult) { return NotFound(); }

            var movieDetailDTO = movieActionResult.Value;
            var selectedGenresIds = movieDetailDTO.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await context.Genres
                .Where(x => !selectedGenresIds.Contains(x.Id))
                .ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieDetailDTO.Movie;
            model.SelectedGenres = movieDetailDTO.Genres;
            model.NotSelectedGenres = notSelectedGenres;
            model.Actors = movieDetailDTO.Actors;
            return model;
        }


        [HttpPost]
        public async Task<ActionResult> Post(Movie movie)
        {
            try
            {
                if (!string.IsNullOrEmpty(movie.Poster))
                {
                    var picture = Convert.FromBase64String(movie.Poster);
                    movie.Poster = await fileStorageService.SaveFile(picture, "jpg", containerName);
                }

                if (movie.MoviesActors != null)
                {
                    for(int i = 0; i < movie.MoviesActors.Count; i++)
                    {
                        movie.MoviesActors[i].Order = i + 1;
                    }
                }

                context.Add(movie);
                await context.SaveChangesAsync();
                return Ok(movie.Id);
            }
            catch(Exception ex)
            {
                return Ok(0);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Movie movie)
        {
            var movieDb = await context.Movies.FindAsync(movie.Id);

            if (movieDb == null)
            {
                return NotFound();
            }

            movieDb = mapper.Map(movie, movieDb);

            if (!string.IsNullOrEmpty(movie.Poster))
            {
                var poster = Convert.FromBase64String(movie.Poster);
                movieDb.Poster = await fileStorageService.EditFile(poster, "jpg", containerName, movieDb.Poster);
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movie.Id}; delete from MoviesGenres where MovieId = {movie.Id}");

            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            movieDb.MoviesActors = movie.MoviesActors;
            movieDb.MoviesGenres = movie.MoviesGenres;

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var m = await context.Movies.FindAsync(id);
            if (m == null)
                return NotFound();

            context.Remove(m);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
