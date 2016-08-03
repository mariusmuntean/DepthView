using System.Threading.Tasks;
using MvvmCross.Plugins.DownloadCache;

namespace DepthViewer.Core.Contracts
{
    public interface IDownloadCache : IMvxFileDownloadCache
    {
        Task<string> GetAndCacheFile(string httpSource);
    }
}