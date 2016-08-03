using System.Collections.Generic;

namespace DepthViewer.Core.Contracts
{
    public interface IDataExchangeService
    {
        Dictionary<string, object> Payload { get; set; }
    }
}