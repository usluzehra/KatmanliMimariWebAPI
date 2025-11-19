using KutuphaneDataAccess.DTOs;
using kutuphaneServis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace UdemyKutuphaneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }


        //program.cs deki rate limiting ayarlarını kullanır
        //[EnableRateLimiting("ReateLimiter")]
        [HttpGet("ListAll")]
        public IActionResult GetAll()
        {
            var books = _bookService.ListAll();
            if (!books.IsSuccess)
            {
                return NotFound(books.Message);
            }
            return Ok(books);
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _bookService.Delete(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] BookCreateDto book)
        {
            if (book == null)
            {
                return BadRequest("Kitap bilgileri boş olamaz");
            }
            var result = _bookService.Create(book);

            if (!result.Result.IsSuccess)
            {
                return BadRequest(result.Result.Message);
            }
            return Ok(result.Result);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            var result = _bookService.GetById(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [HttpGet("GetBooksByCategoryId")]
        public IActionResult GetBooksByCategoryId(int categoryId)
        {
            var result = _bookService.GetBooksByCategoryId(categoryId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [HttpGet("GetBooksByAuthorId")]
        public IActionResult GetBooksByAuthorId(int authorId)
        {
            var result = _bookService.GetAuthorByAuthorId(authorId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] BookUpdateDto bookUpdateDto)
        {
            if (bookUpdateDto == null)
            {
                return BadRequest("Kitap bilgileri boş olamaz");
            }
            var result = _bookService.Update(bookUpdateDto);
            if (!result.Result.IsSuccess)
            {
                return BadRequest(result.Result.Message);
            }
            return Ok(result);
        }
    }
}
