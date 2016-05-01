using DepthViewer.Shared.Models;
using System.Threading.Tasks;

namespace DepthViewer.Shared.Contracts
{
    public interface IParseDataService
    {
        Task<Mapping> GetMapping(string mappingId);
    }
}