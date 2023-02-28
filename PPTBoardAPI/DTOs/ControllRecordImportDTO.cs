namespace PPTBoardAPI.DTOs
{
    public class ControllRecordImportDTO
    {
        public int ControllTypeId { get; set; }
        public int GroupId { get; set; }
        public List<StudentRecord> StudentRecords { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
    public class StudentRecord
    {
        public string FullName { get; set; }
        public List<GradeRecord> Records { get; set; }

    }
    public class GradeRecord
    {
        public string DisciplineName { get; set; }
        public string Grade { get; set; }

    }
}
