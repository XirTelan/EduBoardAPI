using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Service;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/controll")]
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

        //public int GetStatisticById(int id)
        //{
        //    int groupId = 1;
        //    var result = context.ControllRecords.Where(cr => cr.Student.GroupId == groupId && cr.DisciplineId == id && cr.Month == 9 && cr.Year == 2022).Select(cr => cr.Value).Distinct().Count();
        //    return result;
        //}


    }
}
