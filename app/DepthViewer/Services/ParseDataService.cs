using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using DepthViewer.Core.Contracts;
using DepthViewer.Shared.Models;
using MvvmCross.Platform;
using Parse;

namespace DepthViewer.Android.Services
{
    public class ParseDataService : IParseDataService
    {
        private readonly ISecureDataStore _secureDataStore;

        private string _cacheDirPath;
        const string _parseConfigKey = "_parseConfig_";
        const string _parseApiKeyKey = "_parseApiKey_";
        const string _parseNetKeyKey = "_parseNetKey_";
        private bool _initialized = false;

        public ParseDataService(ISecureDataStore secureDataStore)
        {
            _cacheDirPath = Application.Context.FilesDir.Path;
            _secureDataStore = secureDataStore;
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        public void UpdateParseApiKeys(string apiKey, string netKey)
        {
            // Store the new keys in the secure storage
            _secureDataStore.SetProperties(_parseConfigKey, new Dictionary<string, string>()
            {
                {_parseApiKeyKey, apiKey },
                {_parseNetKeyKey, netKey }
            });

            // Init the parse client
            ParseClient.Initialize(apiKey, netKey);
            _initialized = true;
        }

        public void InitializeParse()
        {
            var currentParseConfig = GetCurrentParseConfig();

            if (string.IsNullOrWhiteSpace(currentParseConfig?.ApplicationId)
                || string.IsNullOrWhiteSpace(currentParseConfig.DotNetKey))
            {
                _initialized = false;
                return;
            }

            // Init the parse client
            ParseClient.Initialize(currentParseConfig.ApplicationId,
                                    currentParseConfig.DotNetKey);
            _initialized = true;
        }

        public IParseConfig GetCurrentParseConfig()
        {
            // Look for built-in keys :D
            var parseConfig = Mvx.Resolve<IParseConfig>();

            // Look for stored keys
            var storedParseProps = _secureDataStore.GetProperties(_parseConfigKey);
            if (storedParseProps != null && storedParseProps.ContainsKey(_parseApiKeyKey) &&
                storedParseProps.ContainsKey(_parseNetKeyKey))
            {
                parseConfig.ApplicationId = storedParseProps[_parseApiKeyKey];
                parseConfig.DotNetKey = storedParseProps[_parseNetKeyKey];
            }

            return parseConfig;
        }

        public async Task<List<Mapping>> GetAllMappings()
        {
            var mappings = new List<Mapping>();

            var getAllMappingsQuery = ParseObject.GetQuery("Mapping").Include("measurements");
            var results = await getAllMappingsQuery.FindAsync();

            // Get all local mappings to cross compare
            var localMappingService = Mvx.Resolve<ILocalMappingServices>();
            var localMappingIds = (await localMappingService.GetAllLocalMappings()).Select(m => m.Id);

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
                newLocalMapping.IsSavedLocally = localMappingIds.Contains(mappingId);
                mappings.Add(newLocalMapping);
            }

            return mappings;

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
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        #region Helpers

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

        #endregion
    }
}