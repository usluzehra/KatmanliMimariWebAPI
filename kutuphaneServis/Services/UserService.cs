using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Helpers.ZLog;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using kutuphaneServis.ResponseGeneric;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Services
{
    public class UserService : IUserService
    {
        // DI ile GenericRepositoryi alıyoruz 
        private readonly IGenericRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IZLogger _zLogger;

        public UserService(IGenericRepository<User> userRepository, IConfiguration configuration, IZLogger zLogger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _zLogger = zLogger;
        }
        public IResponse<UserCreateDto> CreateUser(UserCreateDto user)
        {
            if (user == null)
            {
                _zLogger.Error("Kullanıcı bilgileri boş olamaz.");
                ResponseGeneric<UserCreateDto>.Error("Kullanıcı bilgileri boş olamaz.");
            }
            //kullanıcı adı veya şifre boş olamaz

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
            {
                _zLogger.Error("Kullanıcı adı veya email boş olamaz.");
                return ResponseGeneric<UserCreateDto>.Error("Kullanıcı adı veya email boş olamaz.");
            }

            //kullanıcı adı veya e posta adresi zaten var mı kontrol et
            var existingUser = _userRepository.GetAll().FirstOrDefault(x => x.Username == user.Username || x.Email == user.Email);
            
            if(existingUser != null)
            {
                _zLogger.Error("Bu kullanıcı adı veya eposta zaten var.");
                return ResponseGeneric<UserCreateDto>.Error("Bu kullanıcı adı veya eposta zaten var.");
            }

            //gelen şifre alnını hashle
            var hashedPassword = HashedPassword(user.Password);


            //gelen dtoyu entitye dönüştürüyoruz
            var newUser = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Password = hashedPassword 
            };
            newUser.RecordDate = DateTime.Now;
            _userRepository.Create(newUser);
            _zLogger.Info($"Yeni kullanıcı oluşturuldu: {newUser.Username}");
            return ResponseGeneric<UserCreateDto>.Success(null,"Kullanıcı kaydı başarıyla oluşturuldu.");
        }

        public IResponse<string> LoginUser(UserLoginDto user)
        {
            if ((user.Username == null || user.Email== null) && user.Password==null)
            {
                _zLogger.Error("Kullanıcı bilgileri boş olamaz.");
                return ResponseGeneric<string>.Error("Kullanıcı bilgileri boş olamaz.");
            }
          var checkUser=  _userRepository.GetAll().FirstOrDefault(x => (x.Username == user.Username || x.Email == user.Email) && x.Password == HashedPassword(user.Password));


            if (checkUser == null)
            {
                _zLogger.Error("Kullanıcı adı veya şifre hatalı.");
                return ResponseGeneric<string>.Error("Kullanıcı adı veya şifre hatalı.");
            }

            var generatedToken = GenerateJwtToken(checkUser);
            _zLogger.Info($"Kullanıcı girişi başarılı: {checkUser.Username}");
            return ResponseGeneric<string>.Success(generatedToken, "Giriş başarılı.");
        }



        private string HashedPassword(string password)
        {
            string secretKey = "uKI7A:h=&AOv6IX4&[vPgr2:Mu<+Rh";

            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = password + secretKey;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
                var hashedPassword = Convert.ToBase64String(bytes);
                return hashedPassword;
            }

            
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Sid ,user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
