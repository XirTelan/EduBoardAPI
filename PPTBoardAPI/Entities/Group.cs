using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PPTBoardAPI.Entities
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Year { get; set; }
        public int CuratorId { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
        [JsonIgnore]
        public List<Student> Students { get; set; }

    }
}
