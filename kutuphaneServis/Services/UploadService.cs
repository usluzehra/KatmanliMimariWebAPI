using Katmanli.DataAccess.Entities;
using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using kutuphaneServis.ResponseGeneric;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kutuphaneServis.Services
{
    public class UploadImageService : IUploadImageService
    {
        private readonly IGenericRepository<UploadImage> _imageRepo;
        private readonly IGenericRepository<Author> _authorRepo;
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly ILogger<UploadImageService> _logger;

        public UploadImageService(
            IGenericRepository<UploadImage> imageRepo,
            IGenericRepository<Author> authorRepo,
            IGenericRepository<Book> bookRepo,
            ILogger<UploadImageService> logger)
        {
            _imageRepo = imageRepo;
            _authorRepo = authorRepo;
            _bookRepo = bookRepo;
            _logger = logger;
        }

        // 1) UploadFile: IFormFile -> base64 -> UploadImages + Author/Book.ImageFileKey
        public async Task<IResponse<UploadFileResponseDto>> UploadFile(IFormFile file, string entity, int entityId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ResponseGeneric<UploadFileResponseDto>.Error("Dosya bulunamadı.");

                // IFormFile -> base64
                string base64;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    base64 = Convert.ToBase64String(ms.ToArray());
                }

                var fileKey = Guid.NewGuid().ToString("N");

                // UploadImages kaydı
                var upload = new UploadImage
                {
                    FileKey = fileKey,
                    Base64Data = base64,
                    ResimYolu = null
                };
                _imageRepo.Create(upload); // senkron GenericRepository

                // Hedef entity'yi bağla
                var target = (entity ?? string.Empty).Trim().ToLowerInvariant();
                if (target == "author" || target == "yazar")
                {
                    var author = await _authorRepo.GetByIdAsync(entityId);
                    if (author == null)
                        return ResponseGeneric<UploadFileResponseDto>.Error("Yazar bulunamadı.");

                    author.ImageFileKey = fileKey;
                    _authorRepo.Update(author);
                }
                else if (target == "book" || target == "kitap")
                {
                    var book = await _bookRepo.GetByIdAsync(entityId);
                    if (book == null)
                        return ResponseGeneric<UploadFileResponseDto>.Error("Kitap bulunamadı.");

                    book.ImageFileKey = fileKey;
                    _bookRepo.Update(book);
                }
                else
                {
                    return ResponseGeneric<UploadFileResponseDto>.Error("entity 'author' veya 'book' olmalı.");
                }

                _logger.LogInformation("Görsel yüklendi: entity={Entity}, id={Id}, fileKey={Key}", entity, entityId, fileKey);

                return ResponseGeneric<UploadFileResponseDto>.Success(
                    new UploadFileResponseDto { FileKey = fileKey },
                    "Görsel başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadFile hatası. entity={Entity} id={Id}", entity, entityId);
                return ResponseGeneric<UploadFileResponseDto>.Error($"Bir hata oluştu: {ex.Message}");
            }
        }

        // 2) ShowFile: fileKey -> base64 -> bytes
        public async Task<IResponse<ShowFileDto>> ShowFile(string fileKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileKey))
                    return ResponseGeneric<ShowFileDto>.Error("fileKey zorunludur.");

                var img = _imageRepo.GetAll().FirstOrDefault(x => x.FileKey == fileKey);
                if (img == null || string.IsNullOrWhiteSpace(img.Base64Data))
                    return ResponseGeneric<ShowFileDto>.Error("Görsel bulunamadı.");

                byte[] bytes;
                try { bytes = Convert.FromBase64String(img.Base64Data); }
                catch { return ResponseGeneric<ShowFileDto>.Error("Base64 çözülemedi."); }

                var dto = new ShowFileDto
                {
                    Bytes = bytes,
                    ContentType = "image/jpeg",
                    FileName = $"{fileKey}.jpg"
                };

                return ResponseGeneric<ShowFileDto>.Success(dto, "Görsel getirildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ShowFile hatası. fileKey={Key}", fileKey);
                return ResponseGeneric<ShowFileDto>.Error($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
