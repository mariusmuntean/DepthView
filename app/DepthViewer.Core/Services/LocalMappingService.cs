using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DepthViewer.Core.Contracts;
using DepthViewer.Shared.Models;
using MvvmCross.Platform;
using MvvmCross.Plugins.DownloadCache;
using MvvmCross.Plugins.File;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DepthViewer.Core.Services
{
    public class LocalMappingService : ILocalMappingServices
    {
        private IMvxFileStoreAsync _fileStoreAsync;
        private IMvxFileStore _fileStore;
        private IPathProvider _pathProvider;
        private string _baseDir;
        private string _mappingsDir;


        public LocalMappingService()
        {
            _fileStoreAsync = Mvx.Resolve<IMvxFileStoreAsync>();
            _fileStore = Mvx.Resolve<IMvxFileStore>();
            _pathProvider = Mvx.Resolve<IPathProvider>();

            _baseDir = _pathProvider.BaseDirPath;
            _mappingsDir = Path.Combine(_baseDir, "Mappings");

            _fileStore.EnsureFolderExists(_mappingsDir);
        }

		public Task<Mapping> GetMapping (string id)
		{
			var path = Path.Combine(_mappingsDir, id + ".json");
			if (!_fileStore.Exists(path)) {
				return null;
			}

			var mappingJson = string.Empty;
			if (!_fileStore.TryReadTextFile(path, out mappingJson)) {
				return null;
			}

			return Task.Run(() => {
				var mapping = JsonConvert.DeserializeObject<Mapping>(mappingJson);
				mapping.IsSavedLocally = true;
				return mapping;
			});
		}

        public async Task<List<Mapping>> GetAllLocalMappings()
        {
            // Check if Mappings folder exists
            var localMappings = new List<Mapping>();
            if (!_fileStore.FolderExists(_baseDir))
            {
                return localMappings;
            }

            // CheckBox if there are any serialized Mappings inside
            var jsonMappingsPaths = _fileStore.GetFilesIn(_mappingsDir);
            if (jsonMappingsPaths == null || !jsonMappingsPaths.Any())
            {
                return localMappings;
            }

            foreach (var jsonMappingPath in jsonMappingsPaths)
            {
                var id = Path.GetFileNameWithoutExtension(jsonMappingPath);
                var localMapping = await GetMapping(id);
                localMappings.Add(localMapping);
            }

            return localMappings;
        }

        public async Task<Mapping> RefreshLocalMapping(string mappingtId)
        {
            await DeleteLocalMapping(mappingtId);
            var remoteDataService = Mvx.Resolve<IParseDataService>();
            var newerMapping = await remoteDataService.GetMapping(mappingtId);
            await PersistMapping(newerMapping);

            return newerMapping;
        }

        public async Task<List<Mapping>> RefreshAllLocalMappings()
        {
            await DeleteAllLocalMappings();

            var remoteDataService = Mvx.Resolve<IParseDataService>();
            var newerMappings = await remoteDataService.GetAllMappings();
            foreach (var newerMapping in newerMappings)
            {
                await PersistMapping(newerMapping);
            }

            return newerMappings;
        }

        public async Task PersistMapping(Mapping mapping)
        {
            // Persist the Mapping instance as a json
            var jsonMapping = JsonConvert.SerializeObject(mapping);
            var path = Path.Combine(_mappingsDir, mapping.Id + ".json");
            await _fileStoreAsync.WriteFileAsync(path, jsonMapping);

            // Cache the measurement images
            foreach (var measurement in mapping.Measurements)
            {
                await Task.Run(async () =>
                {
                    var downloadTcs = new TaskCompletionSource<string>();
                    Mvx.Resolve<IMvxFileDownloadCache>().RequestLocalFilePath(measurement.ImageUrl, s =>
                    {
                        var downloadPath = Path.Combine(_baseDir, s);
                        downloadTcs.SetResult(true.ToString());
                        Debug.WriteLine("File cached to:{0}", downloadPath);
                    }, exception =>
                    {
                        Debug.WriteLine("Ex: " + exception);
                        downloadTcs.SetException(exception);
                    });

                    await downloadTcs.Task;
                });
            }
        }

        public async Task DeleteAllLocalMappings()
        {
            // Check if Mappings folder exists
            var localMappings = new List<Mapping>();
            if (!_fileStore.FolderExists(_baseDir))
            {
                return;
            }

            // CheckBox if there are any serialized Mappings inside
            var jsonMappingsPaths = _fileStore.GetFilesIn(_mappingsDir);
            if (jsonMappingsPaths == null || !jsonMappingsPaths.Any())
            {
                return;
            }

            // Remove the serialized Mapping objects and their cached images
            var allLocalMappings = await GetAllLocalMappings();
            foreach (var localMapping in allLocalMappings)
            {
                await DeleteLocalMapping(localMapping);
            }
        }

        public async Task DeleteLocalMapping(string mappingId)
        {
            var mappingToDelete = await GetMapping(mappingId);
            await DeleteLocalMapping(mappingToDelete);
        }

        public Task DeleteLocalMapping(Mapping mapping)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Delete serialized Mapping object
                    var path = Path.Combine(_mappingsDir, mapping.Id + ".json");
                    _fileStore.DeleteFile(path);

                    // Delete its cached images
                    foreach (var measurement in mapping.Measurements)
                    {
                        Mvx.Resolve<IMvxFileDownloadCache>().Clear(measurement.ImageUrl);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception deleting {0}: {1}", mapping.Id, ex);
                }
            });
        }
    }
}