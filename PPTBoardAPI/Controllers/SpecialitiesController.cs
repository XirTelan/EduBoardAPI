using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.Controllers
{
    [Route("api/specialities")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public SpecialitiesController(ILogger<SpecialitiesController> logger, ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialityDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Specialities.Include(x => x.SpecialityDiscipline).ThenInclude(x => x.Discipline).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var specialities = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<SpecialityDTO>>(specialities);

        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<SpecialityDTO>>> GetFiltered([FromQuery] string query)
        {
            if (String.IsNullOrEmpty(query))
                return new List<SpecialityDTO>();
            var queryable = context.Specialities.Where(x => x.Name.Contains(query)).Include(x => x.SpecialityDiscipline).ThenInclude(x => x.Discipline).AsQueryable();
            if (queryable == null)
            {
                return new List<SpecialityDTO>();
            }
            var specialities = await queryable.OrderBy(x => x.Name).Take(5).ToListAsync();
            return mapper.Map<List<SpecialityDTO>>(specialities);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<SpecialityDTO>> GetSpecialitiesById(int Id)
        {
            var speciality = await context.Specialities.Include(x => x.SpecialityDiscipline).ThenInclude(x => x.Discipline).FirstOrDefaultAsync(speciality => speciality.Id == Id);
            if (speciality == null)
            {
                return NotFound();
            }
            return mapper.Map<SpecialityDTO>(speciality);
        }
        [HttpGet("edit/{id:int}")]
        public async Task<ActionResult<SpecialityEditDTO>> GetEdit(int id)
        {
            var specialityResult = await GetSpecialitiesById(id);
            if (specialityResult.Result is NotFoundResult) return NotFound();
            var speciality = specialityResult.Value;
            var selectedDisciplinesId = speciality!.Disciplines.Select(x => x.Id).ToList();
            var nonSelectedDisciplines = await context.Disciplines.Where(x => !selectedDisciplinesId.Contains(x.Id)).ToListAsync();
            SpecialityEditDTO response = new SpecialityEditDTO();
            response.speciality = speciality;
            response.selectedDisciplines = speciality.Disciplines;
            response.nonSelectedDisciplines = mapper.Map<List<DisciplineDTO>>(nonSelectedDisciplines);
            return response;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Post([FromBody] SpecialityCreationDTO specialityCreationDTO)
        {
            var speciality = mapper.Map<Speciality>(specialityCreationDTO);
            context.Specialities.Add(speciality);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            var speciality = await context.Specialities.AnyAsync(speciality => speciality.Id == id);
            if (!speciality)
                return NotFound();
            context.Remove(new Speciality() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Put(int id, [FromBody] SpecialityCreationDTO specialityCreationDTO)
        {
            var speciality = await context.Specialities.Include(x => x.SpecialityDiscipline).FirstOrDefaultAsync(speciality => speciality.Id == id);
            if (speciality == null)
            {
                return NotFound();
            }
            speciality = mapper.Map(specialityCreationDTO, speciality);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
