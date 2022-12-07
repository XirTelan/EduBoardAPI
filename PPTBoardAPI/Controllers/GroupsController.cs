using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GroupsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Groups.Include(g => g.Students).Include(x => x.Speciality).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var groups = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GroupDTO>>(groups);

        }
        [HttpGet("filterByYear")]
        public async Task<ActionResult<List<GroupDTO>>> GetByCourse([FromQuery] int course)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            var queryable = context.Groups.Where(g => ((year - Int32.Parse(g.Year)) + 1) == course && month >= 9).Include(g => g.Students).Include(x => x.Speciality).AsQueryable();
            var groups = await queryable.OrderBy(x => x.Name).ToListAsync();
            return mapper.Map<List<GroupDTO>>(groups);

        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<GroupDTO>>> FilterByName([FromQuery] PaginationDTO paginationDTO, [FromQuery] string query)
        {
            var queryable = context.Groups.Where(g => g.Name.Contains(query)).Include(g => g.Students).Include(x => x.Speciality).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var groups = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GroupDTO>>(groups);

        }
        [HttpGet("getall")]

        public async Task<ActionResult<List<GroupDTO>>> GetAll()
        {
            var groups = await context.Groups.Include(g => g.Students).Include(g => g.Speciality).AsQueryable().ToListAsync();
            return mapper.Map<List<GroupDTO>>(groups);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupDTO>> GetById(int id)
        {
            Group? group = await context.Groups.Include(x => x.Speciality).FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
                return NotFound();
            else return mapper.Map<GroupDTO>(group);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GroupCreationDTO groupCreationDTO)
        {
            context.Groups.Add(mapper.Map<Group>(groupCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GroupCreationDTO groupDTO)
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
                return NotFound();
            group = mapper.Map(groupDTO, group);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            Group? group = await context.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
                return NotFound();
            context.Groups.Remove(group);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
