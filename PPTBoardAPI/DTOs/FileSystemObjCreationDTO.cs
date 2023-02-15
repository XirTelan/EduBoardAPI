namespace PPTBoardAPI.DTOs
{
    public class FileSystemObjCreationDTO
    {
        public string DisplayName { get; set; }
        public string? SystemName { get; set; }
        public bool IsFolder { get; set; }
        public int? ParentFolderId { get; set; }
    }
}
