using AutoMapper;
using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using kutuphaneServis.ResponseGeneric;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Services
{
    public class BookService : IBookService
    {
        // DI ile GenericRepositoryi alıyoruz 
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IGenericRepository<Book> bookRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<IResponse<BookCreateDto>> Create(BookCreateDto createBookModel)
        {
            try
            {
            if (createBookModel == null)
            { 
                    return Task.FromResult<IResponse<BookCreateDto>>(ResponseGeneric<BookCreateDto>.Error("Kitap bilgileri boş olamaz"));
            }
            var book = new Book
            {
                Title = createBookModel.Title,
                Description = createBookModel.Description,
                CountOfPage = createBookModel.CountOfPage,
                AuthorId = createBookModel.AuthorId,
                CategoryId = createBookModel.CategoryId,
                //RecordDate = DateTime.Now

            };

                //daha önce eklenmiş mi diye kontrol edelim
                var existingBook = _bookRepository.GetAll().FirstOrDefault(b => b.Title.ToLower() == book.Title.ToLower() && b.AuthorId == book.AuthorId);
                if (existingBook != null)
                {
                    _logger.LogWarning("Aynı isimde ve yazara sahip bir kitap zaten mevcut: {Title}", book.Title);
                    return Task.FromResult<IResponse<BookCreateDto>>(ResponseGeneric<BookCreateDto>.Error("Aynı isimde ve yazara sahip bir kitap zaten mevcut"));
                }

                _bookRepository.Create(book);

                _logger.LogInformation($"Yeni kitap oluşturuldu:" ,book.Title);

                return Task.FromResult<IResponse<BookCreateDto>>(ResponseGeneric<BookCreateDto>.Success(null, "kitap başarıyla oluşturuldu"));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Kitap oluşturulurken bir hata oluştu", createBookModel.Title);
                return Task.FromResult<IResponse<BookCreateDto>>(ResponseGeneric<BookCreateDto>.Error($"Bir hata oluştu: {ex.Message}"));
            }
        }


        public IResponse<Book> Delete(int id)
        {
            try
            {
                //önce entity var mı onu bulacağız

                var book = _bookRepository.GetByIdAsync(id).Result;



                if (book == null)
                {
                    return ResponseGeneric<Book>.Error("Kitap bulunamadı");
                }

                _bookRepository.Delete(book);
                _logger.LogInformation($"Kitap silindi: {book.Title}");
                return ResponseGeneric<Book>.Success(null, "Kitap başarıyla silindi");
            }
            catch
            {
                _logger.LogError($"Kitap silinirken bir hata oluştu: {id}");
                return ResponseGeneric<Book>.Error("Bir hata oluştu");
            }
        }
        public IResponse<BookQuaryDto> GetById(int id)
        {
            try
            {
            var book = _bookRepository.GetByIdAsync(id).Result; 

                var bookDto = _mapper.Map<BookQuaryDto>(book);

                if (bookDto == null)
            {
                return ResponseGeneric<BookQuaryDto>.Success(null,"Kitap bulunamadı");
            }
            return ResponseGeneric<BookQuaryDto>.Success(bookDto, "Kitap başarıyla bulundu");
        }
        catch
            {
                return ResponseGeneric<BookQuaryDto>.Error("Bir hata oluştu");
            }
 }




        public IResponse<IEnumerable<BookQuaryDto>> GetByName(string name)
        {
            try
            {


            var books = _bookRepository.GetAll().Where(x => x.Title == name).ToList();
                 var bookDtos = _mapper.Map<IEnumerable<BookQuaryDto>>(books);


            if (bookDtos==null || bookDtos.ToList().Count==0)
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Kitap bulunamdı.");
            }

            return ResponseGeneric<IEnumerable<BookQuaryDto>>.Success(bookDtos, "Kitap döndürüldü.");
            }
            catch
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bir hata oluştu");
            }
        }


        public IResponse<IEnumerable<BookQuaryDto>> ListAll()
        {
            try
            {        
            var bookList = _bookRepository.GetAll().ToList();
                var bookDtoList = _mapper.Map<IEnumerable<BookQuaryDto>>(bookList);

                if (bookDtoList ==null || bookDtoList.ToList().Count== 0 )
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Kitaplar bulunamdı");
            }
            return ResponseGeneric<IEnumerable<BookQuaryDto>>.Success(bookDtoList, "Kitaplar başarıyla bulundu");

            }
            catch
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bir hata oluştu");
            }

        }


        public IResponse<IEnumerable<BookQuaryDto>> GetBooksByCategoryId(int categoryId)
        {
            try 
            {             
                var booksInCategory = _bookRepository.GetAll().Where(x => x.CategoryId == categoryId).ToList();
                var bookDtos = _mapper.Map<IEnumerable<BookQuaryDto>>(booksInCategory);

                if (bookDtos == null || bookDtos.ToList().Count == 0)
                {
                    return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bu kategoride kitap bulunamadı");
                }

                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Success(bookDtos, "Kitaplar başarıyla bulundu");
            }
            catch
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bir hata oluştu");
            }
        }

        public IResponse<IEnumerable<BookQuaryDto>> GetAuthorByAuthorId(int authorId)
        {
            var booksInAuthor = _bookRepository.GetAll().Where(x => x.AuthorId == authorId).ToList();
            var bookDtos = _mapper.Map<IEnumerable<BookQuaryDto>>(booksInAuthor);
            try
            {     
                if (bookDtos == null || bookDtos.ToList().Count == 0)
                {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bu yazara ait kitap bulunamadı");
                }
            return ResponseGeneric<IEnumerable<BookQuaryDto>>.Success(bookDtos, "Kitaplar başarıyla bulundu");             
            }
            catch
            {
                return ResponseGeneric<IEnumerable<BookQuaryDto>>.Error("Bir hata oluştu");
            }
          
        }

        public Task<IResponse<BookUpdateDto>> Update(BookUpdateDto bookUpdateDto)
        {
            try
            {
                //kitabı dbden bululacağız ilk önce
                var bookEntity = _bookRepository.GetByIdAsync(bookUpdateDto.Id).Result;
                //kitap var mı???
                if (bookEntity == null)
                {
                    return Task.FromResult<IResponse<BookUpdateDto>>(ResponseGeneric<BookUpdateDto>.Error("Kitap bulunamadı"));
                }

                //güncellenecek alanları ayarlayalım
                if (!string.IsNullOrEmpty(bookUpdateDto.Title))
                {
                    bookEntity.Title = bookUpdateDto.Title;
                }
                if (!string.IsNullOrEmpty(bookUpdateDto.Description))
                {
                    bookEntity.Description = bookUpdateDto.Description;
                }
                if (bookUpdateDto.CountOfPage != null)
                {
                    bookEntity.CountOfPage = bookUpdateDto.CountOfPage.Value;
                }
                if (bookUpdateDto.AuthorId != null)
                {
                    bookEntity.AuthorId = bookUpdateDto.AuthorId.Value;
                }
                if (bookUpdateDto.CategoryId != null)
                {
                    bookEntity.CategoryId = bookUpdateDto.CategoryId.Value;
                }

                _bookRepository.Update(bookEntity);
                return Task.FromResult<IResponse<BookUpdateDto>>(ResponseGeneric<BookUpdateDto>.Success(null, "Kitap başarıyla güncellendi"));
            }
            catch
            {
                return Task.FromResult<IResponse<BookUpdateDto>>(ResponseGeneric<BookUpdateDto>.Error("Bir hata oluştu"));

            }
        }


    }
}
