using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
using DepthViewer.Shared.Models;
using Parse;
using System.Diagnostics;

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

        [Route("getMappingPano")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetMappingPano([FromUri] string mappingId)
        {
            try
            {
                var mapping = await GetMapping(mappingId);

                var imageStitcher = IoC.Container.Resolve<IImageStitcher>();

                var resultMemStream = await imageStitcher.StitchImages(mapping.Measurements.Take(10).Select(m => m.ImageUrl).ToList());

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

        public async Task<Mapping> GetMapping(string mappingId)
        {
            Mapping result = null;
            try
            {
                var mappingParseObject = await ParseObject.GetQuery(typeof(Mapping).Name).Include("measurements").GetAsync(mappingId);
                var measurementsParseObjects = mappingParseObject.Get<List<object>>("measurements");
                var localMeasurements = new List<Measurement>();
                foreach (var measurement in measurementsParseObjects)
                {
                    var newLocalMeasurement = await GetMeasurement(measurement as ParseObject);
                    localMeasurements.Add(newLocalMeasurement);
                }

                result = new Mapping(mappingId, new List<Measurement>(localMeasurements), mappingParseObject.CreatedAt.Value);
            }
            catch (Exception ex)
            {
                Trace.TraceError("ImageProcessingController.GetMappingPano(): "+ex.Message);
                Debug.WriteLine("ImageProcessingController.GetMappingPano(): " + ex.Message);
                result = null;
            }

            return result;
        }

        private async Task<Measurement> GetMeasurement(ParseObject parseMeasurement)
        {
            var panAngle = parseMeasurement.Get<double>("panAngle");
            var tiltAngle = parseMeasurement.Get<double>("tiltAngle");
            var distanceCm = parseMeasurement.Get<double>("distanceCm");

            var imageParseFile = parseMeasurement.Get<ParseFile>("image");

            //var downloadTcs = new TaskCompletionSource<string>();

            //Mvx.Resolve<IMvxFileDownloadCache>().RequestLocalFilePath(imageParseFile.Url.AbsoluteUri, s =>
            //{
            //    var downloadPath = Path.Combine(_cacheDirPath, s);
            //    downloadTcs.SetResult(downloadPath);
            //    Console.WriteLine("File cached to:{0}", downloadPath);
            //}, exception =>
            //{
            //    Console.WriteLine("Ex: " + exception);
            //    downloadTcs.SetException(exception);
            //});

            //var imagePath = await downloadTcs.Task;


            return new Measurement(panAngle, tiltAngle, distanceCm, imageParseFile.Url.AbsoluteUri);

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
