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
                var controllRecord = await context.ControllRecords.Where(a => a.StudentId == crDTO.StudentId && a.Year == crDTO.Year && a.Month == crDTO.Month && a.DisciplineId == crDTO.DisciplineId).FirstOrDefaultAsync();
                if (controllRecord == null)
                {
                    context.ControllRecords.Add(mapper.Map<ControllRecord>(crDTO));
                    await context.SaveChangesAsync();
                }
                else if (crDTO.Value == "")
                {
                    context.ControllRecords.Remove(controllRecord);
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (crDTO.Value == controllRecord.Value) continue;
                    controllRecord = mapper.Map(crDTO, controllRecord);
                    await context.SaveChangesAsync();
                }
            }
            return NoContent();
        }
    }
}
