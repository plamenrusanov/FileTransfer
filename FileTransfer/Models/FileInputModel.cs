using System.ComponentModel.DataAnnotations;

namespace FileTransfer.Models
{
    public class FileInputModel
    {
        [Required]
        public IFormFile? File { get; set; }
    }
}
