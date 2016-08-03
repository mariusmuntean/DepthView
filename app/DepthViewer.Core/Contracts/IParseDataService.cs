using System.Collections.Generic;
using System.Threading.Tasks;
using DepthViewer.Shared.Models;

namespace DepthViewer.Core.Contracts
{
    public interface IParseDataService
    {
        bool Initialized { get; }

        void UpdateParseApiKeys(string apiKey, string netKey);
        void InitializeParse();
        IParseConfig GetCurrentParseConfig();

        Task<List<Mapping>> GetAllMappings();

        Task<Mapping> GetMapping(string mappingId);

    }
}