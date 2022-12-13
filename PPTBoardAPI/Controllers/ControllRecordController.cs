using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/controll")]
    public class ControllRecordController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ControllRecordController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ControllGridRowDTO>>> GetByGroipId([FromQuery] int typeId, [FromQuery] int groupId, [FromQuery] int month, [FromQuery] int year)
        {
            List<ControllGridRowDTO> result = new();
            var students = context.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId)?.Students.AsEnumerable();
            if (students == null) return NotFound();
            foreach (var student in students)
            {
                ControllGridRowDTO controllGridRowDTO = new()
                {
                    StudentId = student.Id,
                    StudentFio = $"{student.SecondName} {student.FirstName} {student.MiddleName}",
                    Disciplines = new List<ControllDisciplineValueDTO>()
                };
                var controllRecords = await context.ControllRecords.Where(a => a.ControllTypeId == typeId && a.Student.Id == student.Id && a.Year == year && a.Month == month).ToListAsync();
                if (controllRecords.Any())
                    foreach (var record in controllRecords)
                    {
                        controllGridRowDTO.Disciplines.Add(new ControllDisciplineValueDTO { Id = record.DisciplineId, Value = record.Value });
                    }
                result.Add(controllGridRowDTO);
            }
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecord([FromBody] ControllRecordCreationDTO controllRecordCreationDTO)
        {
            //var controllRecord = await context.Attendances.Where(a => a.StudentId == controllRecordCreationDTO.StudentId && a.Year == controllRecordCreationDTO.Year && a.Month == attendanceCreationDTO.Month && a.Day == attendanceCreationDTO.Day).FirstOrDefaultAsync();
            //if (controllRecord == null)
            //{
            //    context.ControllRecords.Add(mapper.Map<ControllRecord>(controllRecordCreationDTO));
            //    await context.SaveChangesAsync();

            //    return NoContent();
            //}
            //else if (controllRecordCreationDTO.Value == "")
            //{
            //    context.ControllRecords.Remove(controllRecord);
            //    await context.SaveChangesAsync();
            //    return NoContent();

            //}
            //else
            //{
            //    controllRecord = mapper.Map(controllRecordCreationDTO, controllRecord);
            //    await context.SaveChangesAsync();

            return NoContent();
            //}
        }
    }
}
