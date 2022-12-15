namespace PPTBoardAPI.DTOs
{
    public class DataGridRowDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<DataGridCellDTO> DataGridCells { get; set; }
    }
}
