using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.DownloadCache;

namespace DepthViewer.Contracts
{
    public interface IDownloadCache : IMvxFileDownloadCache
    {
        Task<string> GetAndCacheFile(string httpSource);
    }
}