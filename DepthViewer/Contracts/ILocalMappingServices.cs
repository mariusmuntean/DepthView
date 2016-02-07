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
using DepthViewer.Models;

namespace DepthViewer.Contracts
{
    public interface ILocalMappingServices
    {

        Task<Mapping> GetMapping(string id);
        Task<List<Mapping>> GetAllLocalMappings();

        Task PersistMapping(Mapping mapping);
        Task DeleteLocalMapping(string mappingId);

    }
}