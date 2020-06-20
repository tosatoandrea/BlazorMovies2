using AutoMapper;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;

        public PersonController(ApplicationDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO paginationDTO)
        {
            try
            {
                var query = context.Person.AsQueryable();
                await HttpContext.InsertPaginationParametersInResponse<Person>(query, paginationDTO.RecordPerPage);

                return Ok(await query.Paginate(paginationDTO).ToListAsync());
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                return Ok(await context.Person.FindAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult> Get(string searchText)
        {
            try
            {
                if (string.IsNullOrEmpty(searchText))
                    return NotFound(new List<Person>());

                return Ok(await context.Person
                    .Where(x => x.Name.ToLower()
                    .Contains(searchText.ToLower()))
                    .Take(5)
                    .ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Person person)
        {
            if (!string.IsNullOrEmpty(person.Picture))
            {
                var picture = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(picture, "jpg", "people");
            }

            context.Add(person);
            await context.SaveChangesAsync();
            return Ok(person.Id);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            var personDb = await context.Person.FindAsync(person.Id);

            if (personDb  == null)
            {
                return NotFound();
            }

            personDb = mapper.Map(person, personDb);

            if (!string.IsNullOrEmpty(person.Picture))
            {
                var personPicture = Convert.FromBase64String(person.Picture);
                personDb.Picture = await fileStorageService.EditFile(personPicture, "jpg", "people", personDb.Picture);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

       

    }
}
