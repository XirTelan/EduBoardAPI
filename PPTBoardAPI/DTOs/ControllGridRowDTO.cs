namespace PPTBoardAPI.DTOs
{
    public class ControllGridRowDTO
    {
        public int StudentId { get; set; }
        public string StudentFio { get; set; }
        public List<ControllDisciplineValueDTO> Disciplines { get; set; }
    }
}
