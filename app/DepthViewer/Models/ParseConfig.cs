using DepthViewer.Core.Contracts;

namespace DepthViewer.Models
{
    public class ParseConfig : IParseConfig
    {
        private string _applicationId;
        private string _dotNetKey;

        public ParseConfig()
        {
            _applicationId = "";
            _dotNetKey = "";
        }

        public string ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }

        public string DotNetKey
        {
            get { return _dotNetKey; }
            set { _dotNetKey = value; }
        }
    }
}