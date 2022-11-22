using PPTBoardAPI.Entities;

namespace PPTBoardAPI.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string MiddleName { get; set; }
        public Group Group { get; set; }


    }
}
