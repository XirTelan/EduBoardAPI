using PPTBoardAPI.Entities;

namespace PPTBoardAPI.DTOs
{
    public class StudentCreationDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string MiddleName { get; set; }
        public int GroupId { get; set; }


    }
}
