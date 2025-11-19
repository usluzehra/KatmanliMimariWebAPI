using Microsoft.AspNetCore.Mvc;
using KutuphaneDataAccess.DTOs;
using kutuphaneServis.Interfaces;
using System.Security.Claims;

namespace UdemyKutuphaneAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _service;
        public BorrowController(IBorrowService service) => _service = service;

        // NOT: Projendeki JWT payload’a göre düzenle (örnek)
        private int CurrentUserId =>
         int.TryParse(
        User.FindFirst("UserId")?.Value
        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(ClaimTypes.Sid)?.Value,
        out var id)
        ? id
        : 0;

        // Müsaitlik
        [HttpGet("books/{bookId}/availability")]
        public IActionResult Check(int bookId, [FromQuery] DateOnly start, [FromQuery] DateOnly end)
        {
            var result = _service.CheckAvailabilityAsync(bookId, start, end).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // İstek oluştur (user)
        [HttpPost("requests")]
        public IActionResult Create([FromBody] BorrowRequestCreateDto dto)
        {
            var result = _service.CreateRequestAsync(CurrentUserId, dto).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Benim isteklerim
        [HttpGet("requests/my")]
        public IActionResult My()
        {
            var result = _service.MyRequestsAsync(CurrentUserId).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Admin – bekleyen kuyruk
        [HttpGet("requests/pending")]
        public IActionResult Pending()
        {
            var result = _service.PendingQueueAsync().Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Admin – onay
        [HttpPost("{id}/approve")]
        public IActionResult Approve(int id)
        {
            var result = _service.ApproveAsync(CurrentUserId, id).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Admin – reddet
        [HttpPost("{id}/reject")]
        public IActionResult Reject(int id, [FromBody] RejectDto body)
        {
            var result = _service.RejectAsync(CurrentUserId, id, body.Reason).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Admin – teslim et (checkout)
        [HttpPost("{id}/checkout")]
        public IActionResult Checkout(int id)
        {
            var result = _service.CheckoutAsync(CurrentUserId, id).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // Admin – iade al (return)
        [HttpPost("{id}/return")]
        public IActionResult Return(int id)
        {
            var result = _service.ReturnAsync(CurrentUserId, id).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        // User – iptal
        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            var result = _service.CancelAsync(CurrentUserId, id).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        
        [HttpPost("{id}/return/my")]
        public IActionResult ReturnMy(int id)
        {
            var result = _service.ReturnByUserAsync(CurrentUserId, id).Result;
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

    }
}
