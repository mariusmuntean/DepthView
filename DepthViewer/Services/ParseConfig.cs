using DepthViewer.Contracts;

namespace DepthViewer.Services
{
    public class ParseConfig : IParseConfig
    {
        private readonly string _applicationId;
        private readonly string _dotNetKey;

        public ParseConfig()
        {
            _applicationId = "<YourParseAppIdHere>";
            _dotNetKey = "<YourParse.NetKeyHere>";
        }

        public string ApplicationId => _applicationId;

        public string DotNetKey => _dotNetKey;
    }
}