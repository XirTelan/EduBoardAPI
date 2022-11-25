using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class SpecialityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DisciplineDTO> Disciplines { get; set; }
    }
}
