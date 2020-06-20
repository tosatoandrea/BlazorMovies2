using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public RatingController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
        {
            this.context = applicationDbContext;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> Rate(MovieRating movieRating)
        {
            var user = userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            var UserId = user.Id;

            var currentRating = await context.MovieRating
                .FirstOrDefaultAsync(x => x.MovieId == movieRating.MovieId
                && UserId == movieRating.UserId);

            if (currentRating == null)
            {
                movieRating.UserId = UserId;
                movieRating.RatingDate = DateTime.Today;
                context.Add(movieRating);
            }
            else
            {
                currentRating.Rate = movieRating.Rate;
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
