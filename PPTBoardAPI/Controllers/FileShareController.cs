using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel fileModel)
        {
            string path = fileShareService.GetRootFolder();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileInfo fileInfo = new(fileShareService.MakeValidFileName(fileModel.File.FileName));

            if (!fileShareService.IsValidFileType(fileInfo.Extension)) return BadRequest("Invalid File Type");
            string parentPath = String.Empty;
            if (fileModel.ParentFolderID != null)
                parentPath = GetFullFilePath((int)fileModel.ParentFolderID);

            string fullFilePath = Path.Combine(path, parentPath, fileInfo.Name);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullFilePath);
            if (System.IO.File.Exists(fullFilePath))
                return BadRequest("Already exist");

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                fileModel.File.CopyTo(stream);
            }
            context.FileSystemObjs.Add(mapper.Map<FileSystemObj>(new FileSystemObjCreationDTO { DisplayName = fileNameWithoutExtension, IsFolder = false, SystemName = fileInfo.Name, ParentFolderId = fileModel.ParentFolderID }));
            await context.SaveChangesAsync();
            return Ok();
        }
        private string GetFullFilePath(int id)
        {
            string path = BuildPath(id);
            return path;
        }
        private string BuildPath(int id)
        {
            var fileSystemObj = context.FileSystemObjs.Where(fs => fs.Id == id).FirstOrDefault();
            if (fileSystemObj == null) return String.Empty;

            string path = fileSystemObj.IsFolder ? fileSystemObj.SystemName + "\\" : fileSystemObj.SystemName;
            if (fileSystemObj.ParentFolderId != null)
                path = BuildPath((int)fileSystemObj.ParentFolderId) + path;
            return path;
        }
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var fileSystemObj = context.FileSystemObjs.Where(fs => fs.Id == id).FirstOrDefault();
            if (fileSystemObj == null) return BadRequest();
            var filePath = Path.Combine(fileShareService.GetRootFolder(), GetFullFilePath(id));
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "application/octet-stream", fileSystemObj.DisplayName);
        }

        [HttpGet]
        public async Task<List<FileSystemObjView>> GetDirList()
        {
            var fileSystemObjs = await context.FileSystemObjs.Where(fs => fs.ParentFolder == null).OrderBy(fs => !fs.IsFolder).ThenBy(fs => fs.DisplayName).ToListAsync();
            return mapper.Map<List<FileSystemObjView>>(fileSystemObjs);
        }
        [HttpGet("{id:int}")]
        public async Task<List<FileSystemObjView>> GetDirList(int id)
        {
            var fileSystemObjs = await context.FileSystemObjs.Where(fs => fs.ParentFolderId == id).OrderBy(fs => !fs.IsFolder).ThenBy(fs => fs.DisplayName).ToListAsync();
            return mapper.Map<List<FileSystemObjView>>(fileSystemObjs);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDirectory([FromBody] FileSystemObjCreationDTO fileSystemObjCreationDTO)
        {
            if (String.IsNullOrWhiteSpace(fileSystemObjCreationDTO.DisplayName)) return BadRequest();
            string sanitizedFileName = fileShareService.MakeValidFileName(fileSystemObjCreationDTO.DisplayName);

            string parentPath = String.Empty;
            if (fileSystemObjCreationDTO.ParentFolderId != null)
                parentPath = GetFullFilePath((int)fileSystemObjCreationDTO.ParentFolderId);

            string fullPath = Path.Combine(fileShareService.GetRootFolder(), parentPath, sanitizedFileName);
            Directory.CreateDirectory(fullPath);
            fileSystemObjCreationDTO.SystemName = sanitizedFileName;
            fileSystemObjCreationDTO.IsFolder = true;
            context.FileSystemObjs.Add(mapper.Map<FileSystemObj>(fileSystemObjCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFileSystemObject(int id)
        {
            var fileSystemObj = await context.FileSystemObjs.Where(fs => fs.Id == id).FirstOrDefaultAsync();
            if (fileSystemObj == null)
                return NotFound();
            string filePath = Path.Combine(fileShareService.GetRootFolder(), GetFullFilePath(fileSystemObj.Id));


            context.FileSystemObjs.Remove(fileSystemObj);
            await context.SaveChangesAsync();
            if (fileSystemObj.IsFolder) Directory.Delete(filePath, true);
            else System.IO.File.Delete(filePath);

            return NoContent();
        }
    }
}
