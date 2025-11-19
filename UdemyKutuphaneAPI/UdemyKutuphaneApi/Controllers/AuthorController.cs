using KutuphaneCore.DTOs;
using KutuphaneCore.Entities;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UdemyKutuphaneAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        //DI ile servisi inject ettik.
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }



        
        [AllowAnonymous]
        [HttpGet("ListAll")]
        public IActionResult GetAll()
        {
            var authors = _authorService.ListAll();

            if (!authors.IsSuccess)
            {
                return NotFound(authors.Message);
            }


            return Ok(authors);
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _authorService.Delete(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] AuthorCreateDto author)
        {
            if (author == null)
            {
                return BadRequest("yazar bilgileri boş olamaz");
            }
            var result = _authorService.Create(author);

            if (!result.Result.IsSuccess)
            {
                return BadRequest(result.Result.Message);
            }
            return Ok(result);
        }

        [HttpGet("GetByName")]
        public IActionResult GetByName(string name)
        {
            var result = _authorService.GetByName(name);
            if(!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpGet("GetById")]    
        public IActionResult GetById(int id)
        {
            var result = _authorService.GetById(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] AuthorUpdateDto author)
        {
            if (author == null)
            {
                return BadRequest("Yazar bilgileri boş olamaz");
            }
            var result = _authorService.Update(author);
            if (!result.Result.IsSuccess)
            {
                return BadRequest(result.Result.Message);
            }
            return Ok(result);
        }
    }
}
