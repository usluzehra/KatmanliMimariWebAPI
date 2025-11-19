using KutuphaneCore.Entities;

namespace Katmanli.DataAccess.Entities
{
    public class UploadImage : BaseEntity
    {
        public string FileKey { get; set; } = default!;     // Benzersiz kimlik (GUID)
        public string Base64Data { get; set; } = default!;  // Resmin Base64 hali
        public string? ResimYolu { get; set; }              // Şimdilik boş kalacak
    }
}
