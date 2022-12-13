namespace PPTBoardAPI.Entities
{
    public class ControllRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int ControllTypeId { get; set; }
        public ControllType ControllType { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Value { get; set; }
    }
}
