using System.ComponentModel.DataAnnotations;
using PPTBoardAPI.Authentication;

namespace PPTBoardAPI.DTOs
{
    public class GroupDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Year { get; set; }

        public Person Person { get; set; }

        public SpecialityDTO Speciality { get; set; }

        public List<StudentViewDTO> Students { get; set; }
    }
}
