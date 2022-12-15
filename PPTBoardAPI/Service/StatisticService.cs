using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Service
{
    public class StatisticService
    {

        private readonly ApplicationDbContext context;

        public StatisticService(ApplicationDbContext context)
        {

            this.context = context;
        }

        public async Task<List<Discipline>> GetDisciplineListBySpecId(int id)
        {
            var result = new List<Discipline>();
            var speciality = await context.Specialities.Include(s => s.SpecialityDiscipline).ThenInclude(sd => sd.Discipline).Where(s => s.Id == id).FirstOrDefaultAsync();
            if (speciality == null) return result;
            foreach (var record in speciality.SpecialityDiscipline)
            {
                result.Add(record.Discipline);
            }
            return (result);
        }

        public List<DataGridCellDTO> GetGroupStatistic(int groipId, List<ControllRecord> controllRecords)
        {
            List<DataGridRowDTO> resultRows = new();
            List<DataGridCellDTO> result = new();
            List<Discipline> disciplines = GetDisciplineListBySpecId(groipId).Result;
            DataGridRowDTO row = new DataGridRowDTO();
            //row.Id = discipline.Id;
            //row.Title
            //foreach (Discipline discipline in disciplines)
            //{

            //    resultRows. = controllRecords.GroupBy(cr => cr.Value).Select(g => new DataGridCellDTO { Id = g.Key, Value = g.Count().ToString() }).ToList();
            //}
            return result;
        }

        public int GetRecordsCountByValue(string value, List<ControllRecord> controllRecords)
        {
            var result = controllRecords.Where(cr => cr.Value == value).Count();
            return result;
        }



    }
}
