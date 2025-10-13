using kutuphaneServis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UdemyKutuphaneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsControllerSP : ControllerBase
    {
        private readonly IAuthorServiceSP _authorService;
        public AuthorsControllerSP(IAuthorServiceSP authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("ListAllSP")]
        public IActionResult ListAllSP()
        {
            var authors = _authorService.ListAllSP();
            if (!authors.IsSuccess)
            {
                return NotFound(authors.Message);
            }
            return Ok(authors.Data);
        }

        [HttpGet("GetByIdSP")]
        public IActionResult GetByIdSP(int id)
        {
            var author = _authorService.GetByIdSP(id);
            if (!author.IsSuccess)
            {
                return NotFound(author.Message);
            }
            return Ok(author);
        }

        [HttpPost("CreateSP")]
        public IActionResult CreateSP([FromBody] KutuphaneCore.DTOs.AuthorCreateDto author)
        {
            if (author == null)
            {
                return BadRequest("Yazar bilgileri boş olamaz");
            }
            var result = _authorService.CreateSP(author);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPut("UpdateSP")]
        public IActionResult UpdateSP([FromBody] KutuphaneCore.DTOs.AuthorUpdateDto author)
        {
            if (author == null)
            {
                return BadRequest("Yazar bilgileri boş olamaz");
            }
            var result = _authorService.UpdateSP(author);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("DeleteSP")]
        public IActionResult DeleteSP(int id)
        {
            var result = _authorService.DeleteSP(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpGet("GetByNameSP")]
        public IActionResult GetByNameSP(string name)
        {
            var result = _authorService.GetByNameSP(name);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }



    }
}
