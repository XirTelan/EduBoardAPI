namespace PPTBoardAPI.DTOs
{
    public class SpecialityEditDTO
    {
        public SpecialityDTO speciality { get; set; }
        public List<DisciplineDTO> selectedDisciplines { get; set; }
        public List<DisciplineDTO> nonSelectedDisciplines { get; set; }

    }
}
