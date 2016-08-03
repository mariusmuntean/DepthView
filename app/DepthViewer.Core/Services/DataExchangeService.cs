using System.Collections.Generic;
using DepthViewer.Core.Contracts;

namespace DepthViewer.Core.Services
{
    public class DataExchangeService : IDataExchangeService
    {
        public DataExchangeService()
        {
            Payload = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Payload { get; set; }
    }
}