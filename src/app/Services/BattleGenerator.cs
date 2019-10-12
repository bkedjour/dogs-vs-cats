using System;
using System.Linq;
using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;

namespace DogsVsCats.Services
{
    public class BattleGenerator
    {
        private readonly IDataService _dataService;
        private readonly Random _random;

        public BattleGenerator(IDataService dataStore)
        {
            _dataService = dataStore;
            _random = new Random();
        }

        public async Task<Battle> GenerateBattleAsync()
        {
            var dogs = (await _dataService.GetFightersIdsByTypeAsync(FighterType.Dog)).ToList();
            var cats = (await _dataService.GetFightersIdsByTypeAsync(FighterType.Cat)).ToList();

            if (!dogs.Any() || !cats.Any())
                return null;

            return new Battle
            {
                Dog = await _dataService.GetFighterByIdAsync(dogs.ElementAt(_random.Next(dogs.Count))),
                Cat = await _dataService.GetFighterByIdAsync(cats.ElementAt(_random.Next(cats.Count)))
            };
        }
    }
}
