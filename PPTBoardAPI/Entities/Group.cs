using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.Entities
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int SpecialityId { get; set; }
        public int CuratorId { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
    }
}
