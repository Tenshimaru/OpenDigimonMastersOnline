using DigitalWorldOnline.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Services
{
    public interface IDownloadService
    {
        Task<DownloadResult> GetDownloadAsync(string fileName, string architecture);
        bool IsValidDownloadRequest(string fileName, string architecture);
        IEnumerable<DownloadInfo> GetAvailableDownloads();
    }
}
