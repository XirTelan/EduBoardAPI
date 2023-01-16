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
    [ApiController]
    [Route("api/groups")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            var queryable = context.Groups.Include(g => g.Students).Include(x => x.Speciality).Include(g => g.Person).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var groups = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GroupDTO>>(groups);

        }
        [HttpGet("getindexlist")]
        public async Task<ActionResult<List<GroupIndexDTO>>> GetIndexList()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            var queryable = context.Groups.AsQueryable();
            if (queryable == null)
                return NoContent();
            var groups = await queryable.OrderBy(x => x.Name).ToListAsync();
            return mapper.Map<List<GroupIndexDTO>>(groups);

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
            Group? group = await context.Groups.Include(g => g.Speciality).Include(g => g.Person).FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
                return NotFound();
            else return mapper.Map<GroupDTO>(group);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Post([FromBody] GroupCreationDTO groupCreationDTO)
        {
            context.Groups.Add(mapper.Map<Group>(groupCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
