using System.Web.Configuration;
using DepthViewerServer.Contracts;

namespace DepthViewerServer.Services
{
    public class ParseConfig : IParseConfig
    {
        public ParseConfig()
        {
            AppId = WebConfigurationManager.AppSettings["ParseAppId"];
            NetKey = WebConfigurationManager.AppSettings["ParseNetKey"];
        }

        public string AppId { get; }
        public string NetKey { get; }
    }
}