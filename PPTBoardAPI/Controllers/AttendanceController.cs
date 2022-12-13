using AutoMapper;
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

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AttendanceController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<AttendanceGridRowDTO>>> GetByGroipId([FromQuery] int groupId, [FromQuery] int month, [FromQuery] int year)
        {
            List<AttendanceGridRowDTO> result = new();
            var students = context.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId)?.Students.AsEnumerable();
            if (students == null) return NotFound();
            foreach (var student in students)
            {
                AttendanceGridRowDTO attendanceGridRowDTO = new()
                {
                    StudentId = student.Id,
                    StudentFio = $"{student.SecondName} {student.FirstName} {student.MiddleName}",
                    Days = new List<AttendanceDayValueDTO>()
                };
                var attendancesRecords = await context.Attendances.Where(a => a.Student.Id == student.Id && a.Year == year && a.Month == month).ToListAsync();
                if (attendancesRecords.Any())
                    foreach (var record in attendancesRecords)
                    {
                        attendanceGridRowDTO.Days.Add(new AttendanceDayValueDTO { Day = record.Day, Value = record.Value });
                    }
                result.Add(attendanceGridRowDTO);
            }
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecord([FromBody] AttendanceCreationDTO attendanceCreationDTO)
        {
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
                attendanceRecord = mapper.Map(attendanceCreationDTO, attendanceRecord);
                await context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
