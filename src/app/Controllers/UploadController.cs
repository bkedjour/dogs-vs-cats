using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;
using DogsVsCats.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DogsVsCats.Controllers
{
    public class UploadController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;

        public UploadController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors = "All fields are required!";
                return View(model);
            }

            var image = model.Image.OpenReadStream();

            await _dataService.SaveFighterAsync(new Fighter { Name = model.Name },
                            image, model.Image.ContentType, FighterType.Dog);

            return View(new UploadViewModel { IsSuccess = true });
        }
    }
}
