using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DepthViewer.Contracts;

namespace DepthViewer.Services
{
    class DataExchangeService : IDataExchangeService
    {
        public DataExchangeService()
        {
            Payload = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Payload { get; set; }
    }
}