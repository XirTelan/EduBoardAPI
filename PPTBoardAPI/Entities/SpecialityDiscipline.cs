namespace PPTBoardAPI.Entities
{
    public class SpecialityDiscipline
    {
        public int SpecialityId { get; set; }
        public int DisciplineId { get; set; }
        public Speciality Speciality { get; set; }
        public Discipline Discipline { get; set; }
    }
}
