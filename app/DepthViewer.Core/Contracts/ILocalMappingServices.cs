using System.Collections.Generic;
using System.Threading.Tasks;
using DepthViewer.Shared.Models;

namespace DepthViewer.Core.Contracts
{
    public interface ILocalMappingServices
    {

        Task<Mapping> GetMapping(string id);
        Task<List<Mapping>> GetAllLocalMappings();

        Task<Mapping> RefreshLocalMapping(string mappingtId);
        Task<List<Mapping>> RefreshAllLocalMappings();

        Task PersistMapping(Mapping mapping);

        Task DeleteAllLocalMappings();
        Task DeleteLocalMapping(string mappingId);
        Task DeleteLocalMapping(Mapping mapping);

    }
}