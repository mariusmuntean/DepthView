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
using Parse;

namespace DepthViewer.Contracts
{
    interface IParseDataService
    {
        Task<List<Mapping>> GetAllMappings();

        Task<Mapping> GetMapping(string mappingId);

    }
}