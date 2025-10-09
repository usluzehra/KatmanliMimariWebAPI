using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Interfaces
{
    public interface ICategoryService
    {
        IResponse<IEnumerable<CategoryQuaryDto>> ListAll();

        IResponse<CategoryQuaryDto> GetById(int id);
        Task<IResponse<CategoryCreateDto>> Create(CategoryCreateDto category);
        Task<IResponse<CategoryUpdateDto>> Update(CategoryUpdateDto category);
        IResponse<CategoryQuaryDto> Delete(int id);
        IResponse<IEnumerable<CategoryQuaryDto>> GetByName(string name);

    }
}
