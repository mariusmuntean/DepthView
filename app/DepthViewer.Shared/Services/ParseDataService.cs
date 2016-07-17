using System;
using DepthViewer.Shared.Models;
using System.Threading.Tasks;

using Parse;

using System.Collections.Generic;
using DepthViewer.Shared.Contracts;

#if AZURE
using System.Diagnostics;
#endif

namespace DepthViewer.Shared.Services
{
    public class ParseDataService : IParseDataService
    {
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
#if AZURE
                Trace.TraceError("ImageProcessingController.GetMappingPano(): " + ex.Message);
                Debug.WriteLine("ImageProcessingController.GetMappingPano(): " + ex.Message);
#endif
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

            return new Measurement(panAngle, tiltAngle, distanceCm, imageParseFile.Url.AbsoluteUri);
        }
    }
}