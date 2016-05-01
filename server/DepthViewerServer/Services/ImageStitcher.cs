using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DepthViewerServer.Contracts;
using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace DepthViewerServer.Services
{
    public class ImageStitcher : IImageStitcher
    {
        public async Task<Stream> StitchImages(List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
            {
                return null;
            }

            var httpClient = new HttpClient();
            var imageStreams = new List<Stream>();

            foreach (var imageUrl in imageUrls)
            {
                var imageStream = await httpClient.GetStreamAsync(imageUrl);
                imageStreams.Add(imageStream);
            }

            var imageBitmaps = new List<Bitmap>();
            foreach (var imageStream in imageStreams)
            {
                var imageBitmap = new Bitmap(imageStream);
                imageBitmaps.Add(imageBitmap);
            }

            var emguImages = new List<Image<Bgr, byte>>();
            foreach (var imageBitmap in imageBitmaps)
            {
                var image = new Image<Bgr, byte>(imageBitmap);
                emguImages.Add(image);
            }

            var arr = new VectorOfMat();
            foreach (var emguImage in emguImages)
            {
                arr.Push(emguImage.Mat);
            }

            var stitchedImage = new Mat();

            using (var stitcher = new Stitcher(false))
            {
                stitcher.Stitch(arr, stitchedImage);
            }

            var resultMemStream = new MemoryStream();

            stitchedImage.Bitmap.Save(resultMemStream, ImageFormat.Jpeg);
            resultMemStream.Position = 0;

            return resultMemStream;
        }
    }
}