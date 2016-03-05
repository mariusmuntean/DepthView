using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Android.App;
using Android.Content;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using DepthViewer.Contracts;
using DepthViewer.Models;
using Java.IO;
using Parse;
using Console = System.Console;

namespace DepthViewer.Services
{
    public class ParseDataService : IParseDataService
    {
        private string _cacheDirPath;
        public ParseDataService()
        {
            _cacheDirPath = Application.Context.FilesDir.Path;
        }

        public async Task<List<Mapping>> GetAllMappings()
        {
            var mappings = new List<Mapping>();

            var getAllMappingsQuery = ParseObject.GetQuery("Mapping").Include("measurements");
            var results = await getAllMappingsQuery.FindAsync();

            foreach (var mapping in results)
            {
                var measurements = mapping.Get<List<object>>("measurements");
                var localMeasurements = new List<Measurement>();
                foreach (var measurement in measurements)
                {
                    var newLocalMeasurement = await GetMeasurement(measurement as ParseObject);
                    localMeasurements.Add(newLocalMeasurement);
                }

                var mappingId = mapping.ObjectId;
                var newLocalMapping = new Mapping(mappingId, new List<Measurement>(localMeasurements), mapping.CreatedAt.Value);
                mappings.Add(newLocalMapping);
            }

            return mappings;

        }

        #region Helpers

        private async Task<Measurement> GetMeasurement(ParseObject parseMeasurement)
        {
            var panAngle = parseMeasurement.Get<double>("panAngle");
            var tiltAngle = parseMeasurement.Get<double>("tiltAngle");
            var distanceCm = parseMeasurement.Get<double>("distanceCm");

            var imageParseFile = parseMeasurement.Get<ParseFile>("image");
            var downloadTcs = new TaskCompletionSource<string>();


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

        #endregion
    }
}