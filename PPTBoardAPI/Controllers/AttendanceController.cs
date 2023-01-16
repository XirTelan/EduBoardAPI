using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using System.Security.Claims;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AttendanceController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Person> userManager;

        public AttendanceController(ApplicationDbContext context, IMapper mapper, UserManager<Person> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        [HttpGet]
        public async Task<ActionResult<List<DataGridRowDTO>>> GetByGroipId([FromQuery] int groupId, [FromQuery] int month, [FromQuery] int year)
        {
            List<DataGridRowDTO> result = new();
            var students = context.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId)?.Students.AsEnumerable();
            if (students == null) return NotFound();
            foreach (var student in students)
            {
                DataGridRowDTO attendanceGridRowDTO = new()
                {
                    Id = student.Id,
                    Title = $"{student.SecondName} {student.FirstName} {student.MiddleName}",
                    DataGridCells = new List<DataGridCellDTO>()
                };
                var attendancesRecords = await context.Attendances.Where(a => a.Student.Id == student.Id && a.Year == year && a.Month == month).ToListAsync();
                if (attendancesRecords.Any())
                    foreach (var record in attendancesRecords)
                    {
                        attendanceGridRowDTO.DataGridCells.Add(new DataGridCellDTO { Id = record.Day.ToString(), Value = record.Value });
                    }
                result.Add(attendanceGridRowDTO);
            }
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecord([FromBody] AttendanceCreationDTO attendanceCreationDTO)
        {
            bool isInRole = User.IsInRole("Admin") || User.IsInRole("Managment");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = context.Students.Include(s => s.Group).FirstOrDefault(s => s.Id == attendanceCreationDTO.StudentId);
            bool isCurator = student?.Group?.PersonId == userId;
            if (!isInRole && !isCurator) return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Нет прав" });
            var attendanceRecord = await context.Attendances.Where(a => a.StudentId == attendanceCreationDTO.StudentId && a.Year == attendanceCreationDTO.Year && a.Month == attendanceCreationDTO.Month && a.Day == attendanceCreationDTO.Day).FirstOrDefaultAsync();
            if (attendanceRecord == null)
            {
                context.Attendances.Add(mapper.Map<Attendance>(attendanceCreationDTO));
                await context.SaveChangesAsync();

                return NoContent();
            }
            else if (attendanceCreationDTO.Value == "")
            {
                context.Attendances.Remove(attendanceRecord);
                await context.SaveChangesAsync();
                return NoContent();

            }
            else
            {
                if (attendanceCreationDTO.Value == attendanceRecord.Value) return NoContent();
                attendanceRecord = mapper.Map(attendanceCreationDTO, attendanceRecord);
                await context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
