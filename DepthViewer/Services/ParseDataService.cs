using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Cirrious.CrossCore;
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

                var newLocalMapping = new Mapping(new List<Measurement>(localMeasurements), mapping.CreatedAt.Value);
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


            Mvx.Resolve<IMvxFileDownloadCache>().RequestLocalFilePath(imageParseFile.Url.AbsoluteUri, s =>
            {
                downloadTcs.SetResult(s);
                Console.WriteLine("File cached to:{0}", s);
            }, exception =>
            {
                Console.WriteLine("Ex: " + exception);
                downloadTcs.SetException(exception);
            });

            var imagePath = await downloadTcs.Task;


            return new Measurement(panAngle, tiltAngle, distanceCm, imagePath);

        }

        #endregion
    }
}