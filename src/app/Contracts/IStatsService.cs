using System.Collections.Generic;
using System.Threading.Tasks;
using DogsVsCats.Models;

namespace DogsVsCats.Contracts
{
    public interface IStatsService
    {
        Task<IEnumerable<Fighter>> GetStatsAsync();
    }
}
