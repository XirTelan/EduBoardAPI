using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public StudentsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Students.Include(x=>x.Group).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var students = await queryable.OrderBy(x => x.SecondName).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<StudentDTO>>(students);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDTO>> GetById(int id)
        {
            Student? student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            else return mapper.Map<StudentDTO>(student);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentCreationDTO studentCreationDTO)
        {
            context.Students.Add(mapper.Map<Student>(studentCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody] StudentCreationDTO studentDTO)
        {
            var student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            student = mapper.Map(studentDTO,student);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            Student? student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            context.Students.Remove(student);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
