using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;
using PPTBoardAPI.Service;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/disciplines")]
    public class DisciplinesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly StatisticService statisticService;
        private readonly IMapper mapper;

        public DisciplinesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.statisticService = new StatisticService(context);
        }

        [HttpGet]
        public async Task<ActionResult<List<DisciplineDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Disciplines.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var disciplines = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<DisciplineDTO>>(disciplines);

        }
        [HttpGet("getall")]
        public async Task<ActionResult<List<DisciplineDTO>>> GetAll()
        {
            var disciplines = await context.Disciplines.AsQueryable().OrderBy(x => x.Name).ToListAsync();
            return mapper.Map<List<DisciplineDTO>>(disciplines);

        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<DisciplineDTO>>> FilterByName([FromQuery] PaginationDTO paginationDTO, [FromQuery] string query)
        {
            var queryable = context.Disciplines.Where(d => d.Name.Contains(query)).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var disciplines = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<DisciplineDTO>>(disciplines);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DisciplineDTO>> GetById(int id)
        {
            Discipline? discipline = await context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
            if (discipline == null)
                return NotFound();
            else return mapper.Map<DisciplineDTO>(discipline);
        }
        [HttpGet("group/{id:int}")]
        public async Task<ActionResult<List<DisciplineDTO>>> GetByGroupId(int id)
        {
            var group = await context.Groups.Where(g => g.Id == id).FirstOrDefaultAsync();
            if (group == null || group.SpecialityId == null) return NotFound();
            var result = statisticService.GetDisciplineListBySpecId((int)group.SpecialityId);
            if (result.Result.Count == 0)
                return NotFound();
            else return mapper.Map<List<DisciplineDTO>>(result.Result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DisciplineCreationDTO disciplineCreationDTO)
        {
            context.Disciplines.Add(mapper.Map<Discipline>(disciplineCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] DisciplineCreationDTO disciplineDTO)
        {
            var discipline = await context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
            if (discipline == null)
                return NotFound();
            discipline = mapper.Map(disciplineDTO, discipline);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            Discipline? discipline = await context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
            if (discipline == null)
                return NotFound();
            context.Disciplines.Remove(discipline);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
