using KutuphaneDataAccess.DTOs;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using Microsoft.AspNetCore.Mvc;

namespace UdemyKutuphaneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadImageService _uploadService;
        public UploadController(IUploadImageService uploadService) => _uploadService = uploadService;

        // 1) UploadFile — tek FromForm model (Swagger 500 hatasını böyle çözüyoruz)
        [HttpPost("UploadFile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileFormDto form)
        {
            if (!ModelState.IsValid || form.File == null || form.File.Length == 0)
                return BadRequest(ResponseGeneric<string>.Error("Geçerli dosya ve alanlar gerekli."));

            var res = await _uploadService.UploadFile(form.File!, form.Entity!, form.EntityId!.Value);
            if (!res.IsSuccess) return BadRequest(ResponseGeneric<string>.Error(res.Message));

            return Ok(ResponseGeneric<UploadFileResponseDto>.Success(res.Data, res.Message));
        }

        // 2) ShowFile — base64'ü bytes'a çevirip jpg döner
        [HttpGet("ShowFile")]
        public async Task<IActionResult> ShowFile([FromQuery] string fileKey)
        {
            var res = await _uploadService.ShowFile(fileKey);
            if (!res.IsSuccess || res.Data == null)
                return NotFound(ResponseGeneric<string>.Error(res.Message ?? "Görsel bulunamadı."));

            return File(res.Data.Bytes, res.Data.ContentType, res.Data.FileName);
        }
    }
}
