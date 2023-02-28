using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Service;
using System.Security.Claims;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/controll")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ControllRecordController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly StatisticService statisticService;

        public ControllRecordController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.statisticService = new StatisticService(context);
        }

        [HttpGet]
        public async Task<ActionResult<List<DataGridRowDTO>>> GetByGroipId([FromQuery] int typeId, [FromQuery] int groupId, [FromQuery] int month, [FromQuery] int year)
        {
            List<DataGridRowDTO> result = new();
            var students = context.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId)?.Students.AsEnumerable();
            if (students == null) return NotFound();
            var controllRecordsPerPeriod = context.ControllRecords.Include(cr => cr.Student).Where(a => a.ControllTypeId == typeId && a.Year == year && a.Month == month && a.Student.GroupId == groupId);
            foreach (var student in students)
            {
                DataGridRowDTO controllGridRowDTO = new()
                {
                    Id = student.Id,
                    Title = $"{student.SecondName} {student.FirstName} {student.MiddleName}",
                    DataGridCells = new List<DataGridCellDTO>()
                };
                var controllRecords = await controllRecordsPerPeriod.Where(a => a.Student.Id == student.Id).ToListAsync();
                if (controllRecords.Any())
                    foreach (var record in controllRecords)
                    {
                        controllGridRowDTO.DataGridCells.Add(new DataGridCellDTO { Id = record.DisciplineId.ToString(), Value = record.Value });
                    }

                result.Add(controllGridRowDTO);
            }
            return result;
        }
        [HttpGet("statistic/")]
        public async Task<ActionResult<List<DataGridRowDTO>>> GetStatisticByGroipId([FromQuery] int typeId, [FromQuery] int groupId, [FromQuery] int month, [FromQuery] int year)
        {
            var controllRecordsPerPeriod = await context.ControllRecords.Include(cr => cr.Student).Where(a => a.ControllTypeId == typeId && a.Year == year && a.Month == month && a.Student.GroupId == groupId).ToListAsync();
            return statisticService.GetGroupStatistic(groupId, controllRecordsPerPeriod);
        }


        [HttpPost]
        public async Task<ActionResult> CreateRecords([FromBody] List<ControllRecordCreationDTO> controllRecordCreationDTOs)
        {
            bool isInRole = User.IsInRole("Admin") || User.IsInRole("Managment");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var personId = context.Students.Include(s => s.Group).FirstOrDefault(s => s.Id == controllRecordCreationDTOs[0].StudentId)?.Group?.PersonId;
            bool isCurator = personId == userId;
            if (!isInRole && !isCurator) return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Нет прав" });


            foreach (ControllRecordCreationDTO crDTO in controllRecordCreationDTOs)
            {
                await HandleRecord(crDTO);
            }
            return NoContent();
        }


        private async Task<ActionResult> HandleRecord(ControllRecordCreationDTO crDTO)
        {
            var controllRecord = await context.ControllRecords.Where(a => a.StudentId == crDTO.StudentId && a.Year == crDTO.Year && a.Month == crDTO.Month && a.DisciplineId == crDTO.DisciplineId).FirstOrDefaultAsync();
            if (controllRecord == null)
            {
                context.ControllRecords.Add(mapper.Map<ControllRecord>(crDTO));
            }
            else if (crDTO.Value == "")
            {
                context.ControllRecords.Remove(controllRecord);
            }
            else
            {
                if (crDTO.Value != controllRecord.Value)
                    controllRecord = mapper.Map(crDTO, controllRecord);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("import")]
        public async Task<ActionResult> ImportFromExcel([FromBody] ControllRecordImportDTO controllRecordImportDTOs)
        {
            bool isInRole = User.IsInRole("Admin");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var personId = context.Groups.Where(g => g.Id == controllRecordImportDTOs.GroupId).FirstOrDefaultAsync().Result?.PersonId;
            bool isCurator = personId == userId;
            if (!isInRole && !isCurator) return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Нет прав" });

            foreach (StudentRecord studentRecord in controllRecordImportDTOs.StudentRecords)
            {
                if (studentRecord.Records.Count == 0) continue;
                string[] fio = studentRecord.FullName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int studentId = await context.Students.Where(s => s.SecondName == fio[0] && s.FirstName == fio[1]).Select(s => s.Id).FirstOrDefaultAsync();
                if (studentId == 0) continue;
                foreach (GradeRecord gradeRecord in studentRecord.Records)
                {
                    int disciplineId = await context.Disciplines.Where(d => gradeRecord.DisciplineName == d.Name).Select(d => d.Id).FirstOrDefaultAsync();
                    if (disciplineId == 0) continue;
                    ControllRecordCreationDTO controllRecord = new ControllRecordCreationDTO { StudentId = studentId, ControllTypeId = controllRecordImportDTOs.ControllTypeId, DisciplineId = disciplineId, Month = controllRecordImportDTOs.Month, Year = controllRecordImportDTOs.Year, Value = gradeRecord.Grade };
                    await HandleRecord(controllRecord);
                }
            }

            return NoContent();
        }
    }
}
