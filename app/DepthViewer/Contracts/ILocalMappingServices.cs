using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DepthViewer.Shared.Models;

namespace DepthViewer.Contracts
{
    public interface ILocalMappingServices
    {

        Task<Mapping> GetMapping(string id);
        Task<List<Mapping>> GetAllLocalMappings();

        Task<Mapping> RefreshLocalMapping(string mappingtId);
        Task<List<Mapping>> RefreshAllLocalMappings();

        Task PersistMapping(Mapping mapping);

        Task DeleteAllLocalMappings();
        Task DeleteLocalMapping(string mappingId);
        Task DeleteLocalMapping(Mapping mapping);

    }
}