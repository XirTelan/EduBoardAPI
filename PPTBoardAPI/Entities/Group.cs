using PPTBoardAPI.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PPTBoardAPI.Entities
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Year { get; set; }
        public string? PersonId { get; set; }
        public Person? Person { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality? Speciality { get; set; }
        [JsonIgnore]
        public List<Student> Students { get; set; }

    }
}
