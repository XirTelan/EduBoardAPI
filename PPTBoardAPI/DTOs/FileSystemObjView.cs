namespace PPTBoardAPI.DTOs
{
    public class FileSystemObjView
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string SystemName { get; set; }
        public bool IsFolder { get; set; }
        public int ParentFolderId { get; set; }
    }
}
