using System.Collections.Generic;
using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;

namespace DogsVsCats.Services
{
    public class StatsService : IStatsService
    {
        private readonly IDataService _dataService;

        public StatsService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IEnumerable<Fighter>> GetStatsAsync()
        {
             return await _dataService.GetFightersOrderedByVoteAsync();
        }
    }
}
