using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace ReplyApp.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment env)
        {
            this._environment = env;
        }

        public async Task UploadAsync(IFileListEntry fileEntry)
        {
            var path = Path.Combine(_environment.WebRootPath, "Upload", fileEntry.Name);
            var ms = new MemoryStream();
            await fileEntry.Data.CopyToAsync(ms);
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                ms.WriteTo(file); 
            }
        }
    }
}
