using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DepthViewer.Core.Contracts;

//using System.Net.Http;

namespace DepthViewer.Core.Services
{
    public class RemoteImageStitcher : IImageStitcher
    {
        private const string _panoApiUrl = @"http://depthviewer-prod.azurewebsites.net/api/v1/processing/getPano";

        public async Task<byte[]> StitchImages(List<string> imageUrls)
        {
			var httpClient = new HttpClient();
            for(int i=0; i<imageUrls.Count; i++)
            {
                httpClient.DefaultRequestHeaders.Add($"imgUrl{i+1}",imageUrls[i]);
            }
            var response = await httpClient.GetAsync(_panoApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
				
            return imageBytes;
        }
    }
}