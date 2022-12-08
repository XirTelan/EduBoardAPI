
namespace PPTBoardAPI.DTOs
{
    public class AttendanceGridRowDTO
    {
        public int StudentId { get; set; }
        public string StudentFio { get; set; }
        public List<AttendanceDayValueDTO> Days { get; set; }
    }
}
