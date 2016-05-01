using System.Web.Configuration;
using DepthViewerServer.Contracts;

namespace DepthViewerServer.Services
{
    public class HangfireConfig : IHangfireConfig
    {
        private string _connectionString;

        public HangfireConfig()
        {
            _connectionString = WebConfigurationManager.AppSettings["HangfireJobDbConnectionString"];
        }
        public string SqlServerConnectionString
        {
            get { return _connectionString; }
        }
    }
}