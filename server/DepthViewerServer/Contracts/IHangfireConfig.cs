using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DepthViewerServer.Contracts
{
    public interface IHangfireConfig
    {
        string SqlServerConnectionString { get; }
    }
}