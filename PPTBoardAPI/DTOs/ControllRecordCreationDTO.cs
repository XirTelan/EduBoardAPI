namespace PPTBoardAPI.DTOs
{
    public class ControllRecordCreationDTO
    {
        public int StudentId { get; set; }
        public int ControllTypeId { get; set; }
        public int DisciplineId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Value { get; set; }
    }
}
