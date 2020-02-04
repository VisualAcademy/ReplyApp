using BlazorInputFile;
using System.Threading.Tasks;

namespace ReplyApp.Services
{
    public interface IFileUploadService
    {
        Task UploadAsync(IFileListEntry file);
    }
}
