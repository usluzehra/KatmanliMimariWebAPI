using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace KutuphaneDataAccess.DTOs
{
    // Swagger için tek FromForm model
    public class UploadFileFormDto
    {
        [Required] public IFormFile? File { get; set; }   // <input type="file" name="file">
        [Required] public string? Entity { get; set; }     // "author" | "book"
        [Required] public int? EntityId { get; set; }      // ilgili kayıt id
    }

    public class UploadFileResponseDto
    {
        public string FileKey { get; set; } = string.Empty;
    }

    // Controller File(...) dönerken kullanacağımız model
    public class ShowFileDto
    {
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "image/jpeg";
        public string FileName { get; set; } = "image.jpg";
    }
}
