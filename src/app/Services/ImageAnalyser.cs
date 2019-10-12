using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;
using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Logging;

namespace DogsVsCats.Services
{
    public class ImageAnalyser : IImageAnalyser
    {
        private readonly ImageAnnotatorClient _annotator;
        private readonly ILogger _logger;

        public ImageAnalyser(ILoggerFactory loggerFactory)
        {
            _annotator = ImageAnnotatorClient.Create();
            _logger = loggerFactory.CreateLogger("ImageAnalyser");
        }

        public async Task<FighterType?> GetFighterTypeAsync(Stream fighterImage)
        {
            var image = await Image.FromStreamAsync(fighterImage);


            if (!await IsImageSafe(image))
            {
                _logger.LogInformation("User uploaded inappropriate image");
                return null;
            }

            var annotations = await _annotator.DetectLabelsAsync(image);

            if (annotations.Any(a => a.Description.ToLower() == "dog" && a.Score > 0.9f)) return FighterType.Dog;
            if (annotations.Any(a => a.Description.ToLower() == "cat" && a.Score > 0.9f)) return FighterType.Cat;

            _logger.LogInformation($"User uploaded an image different than a dog or a cat. ({annotations?[0].Description})");

            return null;
        }

        private async Task<bool> IsImageSafe(Image image)
        {
            try
            {
                var annotation = await _annotator.DetectSafeSearchAsync(image);

                return annotation.Adult < Likelihood.Possible
                    && annotation.Medical < Likelihood.Possible
                    && annotation.Spoof < Likelihood.Possible
                    && annotation.Violence < Likelihood.Possible;
            }
            catch (AnnotateImageException e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}