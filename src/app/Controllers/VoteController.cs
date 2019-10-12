using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Services;
using DogsVsCats.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DogsVsCats.Controllers
{
    public class VoteController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ILogger _logger;

        public VoteController(IDataService dataStore, ILogger<VoteController> logger)
        {
            _dataService = dataStore;
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromServices]BattleGenerator battleGenerator)
        {
            var battle = await battleGenerator.GenerateBattleAsync();

            if (battle == null)
            {
                _logger.LogWarning("No battle found");

                return View("Empty");
            }

            _logger.LogInformation($"Requesting {battle.Dog.Name} vs {battle.Cat.Name}");

            var model = new VoteViewModel
            {
                DogId = battle.Dog.Id,
                DogName = battle.Dog.Name,
                DogImagePath = battle.Dog.Image,
                CatId = battle.Cat.Id,
                CatName = battle.Cat.Name,
                CatImagePath = battle.Cat.Image
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(string fighterId)
        {
            if (string.IsNullOrEmpty(fighterId))
            {
                _logger.LogTrace("user voted with an empty id");

                return StatusCode(StatusCodes.Status422UnprocessableEntity);
            }

            await _dataService.SetFighterVoteAsync(fighterId);

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            _logger.LogError("Error view sent to the user");
            return View();
        }
    }
}
