namespace PPTBoardAPI.DTOs
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
        public int? ParentFolderID { get; set; }
    }
}
