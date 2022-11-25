using PPTBoardAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class GroupDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Year { get; set; }

        public int CuratorId { get; set; }

        public SpecialityDTO Speciality { get; set; }

        public List<StudentDTO> Students { get; set; } = new List<StudentDTO>();
    }
}
