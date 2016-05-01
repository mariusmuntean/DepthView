using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using DepthViewerServer.Contracts;
using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace DepthViewerServer.Controllers
{
    [RoutePrefix("api/v1/processing")]
    public class ImageProcessingController : ApiController
    {
        [Route("getPano")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPano()
        {
            try
            {
                var headers = HttpContext.Current.Request.Headers;
                var imgUrl1 = headers["imgUrl1"];
                var imgUrl2 = headers["imgUrl2"];

                var imageStitcher = IoC.Container.Resolve<IImageStitcher>();

                var resultMemStream = await imageStitcher.StitchImages(new List<string>() {imgUrl1, imgUrl2});

                var responseMessage = new HttpResponseMessage
                {
                    Content = new StreamContent(resultMemStream)
                    {
                        Headers =
                        {
                            ContentLength = resultMemStream.Length,
                            ContentType = new MediaTypeHeaderValue("image/jpeg"),
                            ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = HttpUtility.UrlDecode("result.jpg"),
                                Size = resultMemStream.Length
                            }
                        }
                    }
                };

                return responseMessage;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = e.Message
                };
            }
        }

        [Route("getPanoDemo")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPanoDemo()
        {
            try
            {
                var imgUrl1 = @"https://cs.brown.edu/courses/csci1950-g/results/proj6/edwallac/source001_01.jpg";
                var imgUrl2 = @"https://cs.brown.edu/courses/csci1950-g/results/proj6/edwallac/source001_02.jpg";

                var img1Stream = await(new HttpClient()).GetStreamAsync(imgUrl1);
                var img2Stream = await(new HttpClient()).GetStreamAsync(imgUrl2);

                var bitmap1 = new Bitmap(img1Stream);
                var bitmap2 = new Bitmap(img2Stream);

                var img1 = new Image<Bgr, byte>(bitmap1);
                var img2 = new Image<Bgr, byte>(bitmap2);

                var arr = new VectorOfMat();
                arr.Push(new[] { img1, img2 });

                var stitchedImage = new Mat();

                using (var stitcher = new Stitcher(false))
                {
                    stitcher.Stitch(arr, stitchedImage);
                }

                var resultMemStream = new MemoryStream();

                stitchedImage.Bitmap.Save(resultMemStream, ImageFormat.Jpeg);
                resultMemStream.Position = 0;

                var responseMessage = new HttpResponseMessage
                {
                    Content = new StreamContent(resultMemStream)
                    {
                        Headers =
                        {
                            ContentLength = resultMemStream.Length,
                            ContentType = new MediaTypeHeaderValue("image/jpeg"),
                            ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = HttpUtility.UrlDecode("result.jpg"),
                                Size = resultMemStream.Length
                            }
                        }
                    }
                };

                return responseMessage;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = e.Message
                };
            }
        }
    }
}
