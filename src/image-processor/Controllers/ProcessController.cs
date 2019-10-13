using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace ImageProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly ILogger<ProcessController> _logger;
        private readonly StorageClient _storage;
        private readonly GcpConfiguration _config;

        private readonly Random random = new Random();

        public ProcessController(ILogger<ProcessController> logger, GcpConfiguration config)
        {
            _logger = logger;
            _config = config;

            _storage = StorageClient.Create();
        }

        [HttpPost]
        public async Task<IActionResult> Process(Payload payload)
        {
            if (payload?.Message == null)
            {
                _logger.LogError("Payload is NULL");
                return BadRequest();
            }

            var fighterName = Encoding.UTF8.GetString(Convert.FromBase64String(payload.Message.Data));
            var rankName = $"rank-{random.Next(1, 13)}.png";

            _logger.LogInformation($"Associating {fighterName} with rank {rankName}");

            var rankFile = new MemoryStream();
            var fighterFile = new MemoryStream();

            await _storage.DownloadObjectAsync(_config.RanksBucketName, rankName, rankFile);
            await _storage.DownloadObjectAsync(_config.FightersBucketName, fighterName, fighterFile);

            var image = Combine(fighterFile, rankFile);

            using (SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100))
            using (Stream outFile = new MemoryStream())
            {
                encoded.SaveTo(outFile);

                await _storage.UploadObjectAsync(_config.FightersBucketName,
                    fighterName,
                    "image/png",
                    outFile,
                    new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });
            }

            return Accepted();
        }

        private SKImage Combine(Stream fighterFile, Stream rankFile)
        {
            SKBitmap fighter = null;
            SKBitmap rank = null;
            try
            {
                fighterFile?.Seek(0, SeekOrigin.Begin);
                rankFile?.Seek(0, SeekOrigin.Begin);

                fighter = SKBitmap.Decode(fighterFile);
                rank = SKBitmap.Decode(rankFile);

                using (var tempSurface = SKSurface.Create(new SKImageInfo(fighter.Width, fighter.Height)))
                {
                    var canvas = tempSurface.Canvas;
                    canvas.Clear(SKColors.Transparent);

                    canvas.DrawBitmap(fighter, SKRect.Create(0, 0, fighter.Width, fighter.Height));
                    canvas.DrawBitmap(rank, SKRect.Create(fighter.Width - rank.Width - 10, 0, rank.Width, rank.Height));

                    return tempSurface.Snapshot();
                }
            }
            finally
            {
                fighter?.Dispose();
                rank?.Dispose();
            }
        }
    }
}
