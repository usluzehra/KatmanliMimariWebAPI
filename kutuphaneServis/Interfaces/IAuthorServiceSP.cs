using KutuphaneCore.DTOs;
using KutuphaneCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Interfaces
{
    public interface IAuthorServiceSP
    {
        IResponse<IEnumerable<AuthorQuaryDto>> ListAllSP();

        IResponse<AuthorQuaryDto> GetByIdSP(int id);
        IResponse<AuthorCreateDto> CreateSP(AuthorCreateDto author);
        IResponse<AuthorUpdateDto> UpdateSP(AuthorUpdateDto author);
        IResponse<AuthorQuaryDto> DeleteSP(int id);
        IResponse<IEnumerable<AuthorQuaryDto>> GetByNameSP(string name);
    }
}
