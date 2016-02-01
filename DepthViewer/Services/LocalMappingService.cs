using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.MvvmCross.Plugins.FileSystem;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using DepthViewer.Contracts;
using DepthViewer.Models;
using Newtonsoft.Json;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

namespace DepthViewer.Services
{
    public class LocalMappingService : ILocalMappingServices
    {
        private IMvxFileStoreAsync _fileStoreAsync;
        private IMvxFileStore _fileStore;
        private string _baseDir;
        private string _mappingsDir;


        public LocalMappingService()
        {
            _fileStoreAsync = Mvx.Resolve<IMvxFileStoreAsync>();
            _fileStore = Mvx.Resolve<IMvxFileStore>();

            _baseDir = Application.Context.FilesDir.Path;
            _mappingsDir = Path.Combine(_baseDir, "Mappings");

            _fileStore.EnsureFolderExists(_mappingsDir);
        }

        public Task<Mapping> GetMapping(string id)
        {
            var path = Path.Combine(_mappingsDir, id + ".json");
            if (!_fileStore.Exists(path))
            {
                return null;
            }

            var mappingJson = string.Empty;
            if (!_fileStore.TryReadTextFile(path, out mappingJson))
            {
                return null;
            }

            return Task.Run(() => JsonConvert.DeserializeObject<Mapping>(mappingJson));
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

        public async Task PersistMapping(Mapping mapping)
        {
            var jsonMapping = JsonConvert.SerializeObject(mapping);
            var path = Path.Combine(_mappingsDir, mapping.Id + ".json");
            await _fileStoreAsync.WriteFileAsync(path, jsonMapping);
        }

        public Task DeleteLocalMapping(string mappingId)
        {
            var path = Path.Combine(_mappingsDir, mappingId + ".json");

            return Task.Run(() =>
            {
                try
                {
                    _fileStore.DeleteFile(path);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception deleting {0}: {1}", mappingId, ex);
                }
            });
        }
    }
}