using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly UserManager<Person> userManager;
        private readonly SignInManager<Person> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AttendanceController(UserManager<Person> userManager, SignInManager<Person> signInManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<AttendanceDTO>>> Get()
        {
            var queryable = await context.Attendances.ToListAsync();
            List<AttendanceDTO> result = mapper.Map<List<AttendanceDTO>>(queryable);
            return result;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetByGroipId(int id)
        {
            var students = context.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == id)?.Students.AsEnumerable();
            var queryable = await context.Attendances.Where(a => students!.Contains(a.Student)).ToListAsync();
            List<AttendanceDTO> result = mapper.Map<List<AttendanceDTO>>(queryable);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecord([FromForm] AttendanceCreationDTO attendanceCreationDTO)
        {
            var attendanceRecord = await context.Attendances.Where(a => a.StudentId == attendanceCreationDTO.StudentId && a.Year == attendanceCreationDTO.Year && a.Month == attendanceCreationDTO.Month && a.Day == attendanceCreationDTO.Day).FirstOrDefaultAsync();
            if (attendanceRecord == null)
            {
                context.Attendances.Add(mapper.Map<Attendance>(attendanceCreationDTO));
                await context.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                attendanceRecord = mapper.Map(attendanceCreationDTO, attendanceRecord);
                await context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
