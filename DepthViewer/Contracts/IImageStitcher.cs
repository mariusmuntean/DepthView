using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DepthViewer.Contracts
{
    public interface IImageStitcher
    {
        Task<byte[]> StitchImages(List<string> imageUrls);
    }
}