using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class GroupCreationDTO
    {
        [Required]
        public string Name { get; set; }
        public int SpecialityId { get; set; }
        public string Year { get; set; }
        public string? PersonId { get; set; }
        public List<int> StudentsId { get; set; } = new List<int>();
    }
}
