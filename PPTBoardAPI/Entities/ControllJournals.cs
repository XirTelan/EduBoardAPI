namespace PPTBoardAPI.Entities
{
    public class ControllJournals
    {
        public int Id { get; set; }
        public int StudId { get; set; }
        public Student Student { get; set; }
        public string Type { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Value { get; set; }
    }
}
