using AutoMapper;
using KutuphaneCore.DTOs;
using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Helpers.ZLog;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using kutuphaneServis.ResponseGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
   Amacı: Yazar (Author) nesneleri üzerinde iş kurallarını çalıştırmak ve Repository üzerinden DB ile iletişim kurmak.
 */

namespace kutuphaneServis.Services
{
    public class AuthorService : IAuthorService

    {
        // DI ile GenericRepositoryi alıyoruz 
        private readonly IGenericRepository<Author> _authorRepository;
        private readonly IMapper _mapper;
        private readonly IZLogger _zLogger;

        public AuthorService(IGenericRepository<Author> authorRepository, IMapper mapper, IZLogger zLogger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _zLogger = zLogger;
        }

     
        public  Task<IResponse<Author>> Create(AuthorCreateDto authorCreateDto)
        {
            try
            {
                 if(authorCreateDto == null)
                 {
                    _zLogger.Error("Yazar bilgileri boş olamaz.");
                    return Task.FromResult<IResponse<Author>>(ResponseGeneric<Author>.Error("Yazar bilgileri boş olamaz."));
              
                 }
                var newAuthor = _mapper.Map<Author>(authorCreateDto);

                newAuthor.RecordDate= DateTime.Now;

                 _authorRepository.Create(newAuthor);

                    _zLogger.Info($"Yeni yazar oluşturuldu: {newAuthor.Name} {newAuthor.Surname}");

                return Task.FromResult<IResponse<Author>>(ResponseGeneric<Author>.Success(null, "Yazar başarıyla oluşturuldu."));
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex,"Yazar oluşturulurken bir hata oluştu.");
                return Task.FromResult<IResponse<Author>>(ResponseGeneric<Author>.Error(ex.Message));
            }           
           
        }

        public IResponse<Author> Delete(int id)
        {
            try
            {
                  //önce entity var mı onu bul 
                    var author =_authorRepository.GetByIdAsync(id).Result;


                    if (author == null)
                    {
                        _zLogger.Warn("Silinecek yazar bulunamadı.");
                    return ResponseGeneric<Author>.Error("Yazar bulunamadı.");
                    }
                    // Entity varsa sil
                    _authorRepository.Delete(author);
                    _zLogger.Info($"Yazar silindi: {author.Name} {author.Surname}");
                return ResponseGeneric<Author>.Success(null, "Yazar başarıyla silindi.");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Yazar silinirken bir hata oluştu.");
                {
                    return ResponseGeneric<Author>.Error(ex.Message);
                }
            }        
        }

        public IResponse<AuthorQuaryDto> GetById(int id)
        {
            try
            {
            var author= _authorRepository.GetByIdAsync(id).Result;
                var authorQuaryDto = _mapper.Map<AuthorQuaryDto>(author);
                if (author == null)
               {
                    _zLogger.Warn("Yazar bulunamadı.");
                    return ResponseGeneric<AuthorQuaryDto>.Error("Yazar bulunamadı.");
               }
                _zLogger.Info($"Yazar bulundu: {author.Name} {author.Surname}");
                return ResponseGeneric<AuthorQuaryDto>.Success(authorQuaryDto, "Yazar başarıyla bulundu.");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Yazar getirilirken bir hata oluştu.");
                {
                    return ResponseGeneric<AuthorQuaryDto>.Error(ex.Message);
                }
            }
        }

        public IResponse<IEnumerable<AuthorQuaryDto>> GetByName(string name)
        {
            try
            {
                var authorList= _authorRepository.GetAll().Where(x=>x.Name==name).ToList();
                var authorQuaryDtos = _mapper.Map<IEnumerable<AuthorQuaryDto>>(authorList);

                if (authorQuaryDtos == null || authorQuaryDtos.ToList().Count ==0)
                {
                    _zLogger.Warn("İsimle yazar bulunamadı.");
                    return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error("yazar bulunamadı");
                }
                _zLogger.Info($"İsimle yazar bulundu: {name}");
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Success(authorQuaryDtos, "yazar başarıyla bulundu.");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "İsimle yazar getirilirken bir hata oluştu.");

                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error(ex.Message);
            }
            
        }

        public IResponse<IEnumerable<AuthorQuaryDto>> ListAll()
        {
            try
            {
                var allAuthors = _authorRepository.GetAll().ToList();
                var authorQueryDtos = _mapper.Map<IEnumerable<AuthorQuaryDto>>(allAuthors);

                if (allAuthors == null || allAuthors.Count == 0)
                {
                    _zLogger.Warn("Yazarlar bulunamadı.");
                    return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error("yazarlar bulunamadı");
                }

                _zLogger.Info("Tüm yazarlar listelendi.");
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Success(authorQueryDtos, "yazarlar bulundu");

            }
            catch (Exception ex)
            {
                _zLogger.Error(ex, "Yazarlar getirilirken bir hata oluştu.");
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error(ex.Message);
            }

        }

        public Task<IResponse<AuthorUpdateDto>> Update(AuthorUpdateDto authorUpdateDto)
        {
            try
            { 
                //önce db de var mı 
                var authorEntity = _authorRepository.GetByIdAsync(authorUpdateDto.Id).Result;
                if (authorEntity == null)
                {
                    _zLogger.Warn("Güncellenecek yazar bulunamadı.");
                    return Task.FromResult<IResponse<AuthorUpdateDto>>(ResponseGeneric<AuthorUpdateDto>.Error("Yazar bulunamadı."));
                }
                //var olan entityi güncelle
                if(!string.IsNullOrEmpty(authorUpdateDto.Name))
                {
                    authorEntity.Name = authorUpdateDto.Name;
                }
                if(!string.IsNullOrEmpty(authorUpdateDto.Surname))
                {
                    authorEntity.Surname = authorUpdateDto.Surname;
                }
               if(!string.IsNullOrEmpty(authorUpdateDto.PlaceOfBirth))
                {
                    authorEntity.PlaceOfBirth = authorUpdateDto.PlaceOfBirth;
                }
                if(authorUpdateDto.YearOfBirth != null)
                {
                    authorEntity.YearOfBirth = authorUpdateDto.YearOfBirth.Value;
                }
                _authorRepository.Update(authorEntity);
                _zLogger.Info($"Yazar güncellendi: {authorEntity.Name} {authorEntity.Surname}");
                return Task.FromResult<IResponse<AuthorUpdateDto>>(ResponseGeneric<AuthorUpdateDto>.Success(null, "Yazar başarıyla güncellendi."));     
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Yazar güncellenirken bir hata oluştu.");
                
                return Task.FromResult<IResponse<AuthorUpdateDto>>(ResponseGeneric<AuthorUpdateDto>.Error(ex.Message));
            }
            

        }
    }
}
