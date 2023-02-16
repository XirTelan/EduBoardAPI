namespace PPTBoardAPI.Service
{
    public class FileShareService
    {
        public string GetRootFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string parentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;

            string filePath = Path.Combine(parentDirectory, "uploads");
            return filePath;
        }

        public string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public bool IsValidFileType(string fileExtension)
        {
            string[] allowedFileTypes = { ".pdf", ".doc", ".docx", ".xlsx" };
            if (!allowedFileTypes.Contains(fileExtension))
            {
                return false;
            }
            return true;
        }
    }
}
