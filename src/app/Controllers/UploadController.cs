using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;
using DogsVsCats.ViewModels;
using Google.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DogsVsCats.Controllers
{
    public class UploadController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IImageAnalyser _imageDescriptor;
        private readonly GcpConfiguration _config;

        private readonly ILogger _logger;

        public UploadController(IDataService dataService, IImageAnalyser imageDescriptor, GcpConfiguration config, ILogger<UploadController> logger)
        {
            _dataService = dataService;
            _imageDescriptor = imageDescriptor;
            _config = config;
            _logger = logger;
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

            var fighterType = await _imageDescriptor.GetFighterTypeAsync(image);

            if (!fighterType.HasValue)
            {
                model.Errors = "Please upload an image of either a dog or a cat";
                return View(model);
            }

            var fighterId = await _dataService.SaveFighterAsync(new Fighter { Name = model.Name },
                            image, model.Image.ContentType, fighterType.Value);

            TopicName topicName = new TopicName(_config.ProjectId, _config.Topic);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);
            string messageId = await publisher.PublishAsync(fighterId);

            _logger.LogInformation($"Published message : {messageId}");

            return View(new UploadViewModel { IsSuccess = true });
        }
    }
}
