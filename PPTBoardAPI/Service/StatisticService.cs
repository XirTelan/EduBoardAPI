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

        public List<DataGridRowDTO> GetGroupStatistic(int groipId, List<ControllRecord> controllRecords)
        {
            List<DataGridRowDTO> resultRows = new();
            var dsiciplines = GetDisciplineListBySpecId(1).Result;
            foreach (var dsicipline in dsiciplines)
            {
                DataGridRowDTO dataGridRow = new()
                {
                    Id = dsicipline.Id,
                    Title = dsicipline.Name,
                    DataGridCells = controllRecords.Where(cr => cr.DisciplineId == dsicipline.Id).GroupBy(cr => cr.Value).Select(g => new DataGridCellDTO { Id = g.Key, Value = g.Count().ToString() }).ToList()
                };
                resultRows.Add(dataGridRow);
            }
            //dataGridRow.DataGridCells = controllRecords.GroupBy(cr => cr.Value).Select(g => new DataGridCellDTO { Id = g.Key, Value = g.Count().ToString() }).ToList();
            return resultRows;
        }




    }
}
