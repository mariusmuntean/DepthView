using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DepthViewerServer.Contracts
{
    public interface IImageStitcher
    {
        Task<Stream> StitchImages(List<string> imageUrls);
    }
}