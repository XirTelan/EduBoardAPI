using PPTBoardAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class GroupCreationDTO
    {
        [Required]
        public string Name { get; set; }
        public int SpecialityId { get; set; }
        public int CuratorId { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
    }
}
