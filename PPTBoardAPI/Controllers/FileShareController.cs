using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Service;


namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileShareController : ControllerBase
    {
        private readonly FileShareService fileShareService;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public FileShareController(ApplicationDbContext context, IMapper mapper)
        {
            this.fileShareService = new FileShareService();
            this.context = context;
            this.mapper = mapper;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(FileUploadModel fileModel)
        {
            string path = fileShareService.GetRootFolder();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileInfo fileInfo = new(fileModel.file.FileName);
            string fileName = fileModel.file.Name + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                fileModel.file.CopyTo(stream);
            }
            fileModel.fileInformation.SystemName = fileName;
            context.FileSystemObjs.Add(mapper.Map<FileSystemObj>(fileModel.fileInformation));
            await context.SaveChangesAsync();
            return NoContent();
        }
        private string GetFullFilePath(int id)
        {
            return ''
        }
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var filePath = Path.Combine(fileShareService.GetRootFolder(), "files/");
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }

        [HttpGet]
        public async Task<List<FileSystemObjView>> GetDirList()
        {
            var fileSystemObjs = await context.FileSystemObjs.Where(fs => fs.ParentFolder == null).ToListAsync();
            return mapper.Map<List<FileSystemObjView>>(fileSystemObjs);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDirectory(FileSystemObjCreationDTO fileSystemObjCreationDTO)
        {
            string fullPath = Path.Combine(fileShareService.GetRootFolder(), fileSystemObjCreationDTO.DisplayName);
            Directory.CreateDirectory(fullPath);
            fileSystemObjCreationDTO.SystemName = fileSystemObjCreationDTO.DisplayName;
            context.FileSystemObjs.Add(mapper.Map<FileSystemObj>(fileSystemObjCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<List<FileSystemObjView>> GetDirList(int id)
        {
            var fileSystemObjs = await context.FileSystemObjs.Where(fs => fs.ParentFolderId == id).ToListAsync();
            return mapper.Map<List<FileSystemObjView>>(fileSystemObjs);
        }

    }
}
