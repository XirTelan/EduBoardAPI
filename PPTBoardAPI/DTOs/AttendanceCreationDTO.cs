namespace PPTBoardAPI.DTOs
{
    public class AttendanceCreationDTO
    {
        public int StudentId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Value { get; set; }
    }
}
