using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DogsVsCats.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public string Errors { get; set; }

        public bool IsSuccess { get; set; }
    }
}
