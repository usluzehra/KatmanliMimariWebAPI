using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Interfaces
{
    public interface IUserService
    {
        public IResponse<UserCreateDto> CreateUser(UserCreateDto user);
        public IResponse<string> LoginUser(UserLoginDto user);

    }
}
