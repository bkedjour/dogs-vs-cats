using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DogsVsCats.Models;

namespace DogsVsCats.Contracts
{
    public interface IDataService
    {
        Task<IEnumerable<string>> GetFightersIdsByTypeAsync(FighterType type);

        Task<Fighter> GetFighterByIdAsync(string id);

        Task SetFighterVoteAsync(string id);

        Task SaveFighterAsync(Fighter fighter, Stream image, string contentType, FighterType type);

        Task<IEnumerable<Fighter>> GetFightersOrderedByVoteAsync();
    }
}
