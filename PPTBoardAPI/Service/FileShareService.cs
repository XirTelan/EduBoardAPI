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
    }
}
