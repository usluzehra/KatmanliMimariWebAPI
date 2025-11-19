using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UdemyKutuphaneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        public UserController(IUserService userService, IGenericRepository<User> userReposıtory, IGenericRepository<Role> roleRepository, IGenericRepository<UserRole> userRoleRepository)
        {
            _userService = userService;
            _userRepository = userReposıtory;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }
        [HttpPost("Create")]
        public IActionResult CreateUser([FromBody] UserCreateDto user)
        {
           if(user == null)
            {
                return BadRequest("Kullanıcı bilgileri boş olamaz.");
            }
            var result = _userService.CreateUser(user);
            if(result.IsSuccess)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("Login")]
        public IActionResult LoginUser([FromBody] UserLoginDto user)
        {
            if (user == null)
            {
                return BadRequest("Kullanıcı bilgileri boş olamaz.");
            }
            var result = _userService.LoginUser(user);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

    

      // ------------------- ADMIN işlemleri -------------------

        [Authorize(Roles = "Admin")]
        [HttpGet("ListUsers")]
        public IActionResult ListUsers()
        {
            var users = _userRepository.GetAll().ToList();
            var roles = _roleRepository.GetAll().ToList();
            var userRoles = _userRoleRepository.GetAll().ToList();

            var list = users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email!,
                Roles = userRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            }).ToList();

            return Ok(ResponseGeneric<List<UserListItemDto>>.Success(list));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignRole")]
        public IActionResult AssignRole([FromBody] AssignRoleRequest req)
        {
            var user = _userRepository.GetAll().FirstOrDefault(x => x.Id == req.UserId);
            if (user == null)
                return BadRequest(ResponseGeneric<string>.Error("Kullanıcı bulunamadı."));

            var role = _roleRepository.GetAll().FirstOrDefault(x => x.Name == req.RoleName);
            if (role == null)
                return BadRequest(ResponseGeneric<string>.Error("Rol bulunamadı."));

            var exists = _userRoleRepository.GetAll().Any(x => x.UserId == user.Id && x.RoleId == role.Id);
            if (exists)
                return BadRequest(ResponseGeneric<string>.Error("Kullanıcının zaten bu rolü var."));

            _userRoleRepository.Create(new UserRole { UserId = user.Id, RoleId = role.Id });
            return Ok(ResponseGeneric<string>.Success(null, $"{user.Username} kullanıcısına {role.Name} rolü eklendi."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RemoveRole")]
        public IActionResult RemoveRole([FromBody] AssignRoleRequest req)
        {
            var role = _roleRepository.GetAll().FirstOrDefault(x => x.Name == req.RoleName);
            if (role == null)
                return BadRequest(ResponseGeneric<string>.Error("Rol bulunamadı."));

            var userRole = _userRoleRepository.GetAll().FirstOrDefault(x => x.UserId == req.UserId && x.RoleId == role.Id);
            if (userRole == null)
                return BadRequest(ResponseGeneric<string>.Error("Kullanıcının bu rolü yok."));

            _userRoleRepository.Delete(userRole);
            return Ok(ResponseGeneric<string>.Success(null, $"{req.RoleName} rolü kullanıcıdan kaldırıldı."));
        }

        [Authorize]
        [HttpGet("Debug/Claims")]
        public IActionResult WhoAmI()
        {
            var claims = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();
            return Ok(claims);
        }

    }

    // DTO: aynı dosyaya veya ayrı DTO klasörüne koyabilirsin
    public class AssignRoleRequest
{
    public int UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

   
}

