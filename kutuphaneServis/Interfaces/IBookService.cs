using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Interfaces
{
    public interface IBookService 
    {
        IResponse<IEnumerable<BookQuaryDto>> ListAll();

        IResponse<BookQuaryDto> GetById(int id);
        Task<IResponse<BookCreateDto>> Create(BookCreateDto book);
        Task<IResponse<BookUpdateDto>> Update(BookUpdateDto bookUpdateDto);
        IResponse<Book> Delete(int id);
        IResponse<IEnumerable<BookQuaryDto>> GetByName(string name);

        IResponse<IEnumerable<BookQuaryDto>> GetBooksByCategoryId(int categoryId);
        IResponse<IEnumerable<BookQuaryDto>> GetAuthorByAuthorId(int authorId);


    }
}
