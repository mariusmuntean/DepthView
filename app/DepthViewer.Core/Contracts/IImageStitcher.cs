using System.Collections.Generic;
using System.Threading.Tasks;

namespace DepthViewer.Core.Contracts
{
    public interface IImageStitcher
    {
        Task<byte[]> StitchImages(List<string> imageUrls);
    }
}