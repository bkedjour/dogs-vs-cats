using System.Linq;
using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DogsVsCats.Controllers
{
    public class StatsController : Controller
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _statsService.GetStatsAsync();

            return View(new StatsViewModel { Fighters = stats.ToList() });
        }
    }
}