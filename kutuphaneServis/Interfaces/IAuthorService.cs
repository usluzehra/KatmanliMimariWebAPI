using KutuphaneCore.DTOs;
using KutuphaneCore.Entities;
using kutuphaneServis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//nasıl yaptığını söylemeden her iş alanı için hangi operasyonları sunduğunu imzalarla tarif etmek
namespace kutuphaneServis.Interfaces
{
    public interface IAuthorService
    {
        IResponse<IEnumerable<AuthorQuaryDto>> ListAll();

        IResponse<AuthorQuaryDto> GetById(int id);
        Task<IResponse<Author>> Create(AuthorCreateDto author);
        Task<IResponse<AuthorUpdateDto>> Update(AuthorUpdateDto author);
        IResponse<Author> Delete(int id);
        IResponse<IEnumerable<AuthorQuaryDto>> GetByName(string name);

    }
}
