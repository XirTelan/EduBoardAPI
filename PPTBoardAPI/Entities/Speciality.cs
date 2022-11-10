using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.Entities
{
    public class Speciality
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
