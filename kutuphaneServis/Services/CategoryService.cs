using AutoMapper;
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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IZLogger _zLogger;


        public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper, IZLogger zLogger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _zLogger = zLogger;
        }
        public async Task<IResponse<CategoryCreateDto>> Create(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                if (categoryCreateDto == null)
                {
                    _zLogger.Error("kategori bilgileri boş olamaz");
                    return ResponseGeneric<CategoryCreateDto>.Error("kategori bilgileri boş olamaz");
                }

                //DTOyu entitye maple
                var categoryEntity = new Category{ Name = categoryCreateDto.Name,Description = categoryCreateDto.Description };
                categoryEntity.RecordDate = DateTime.Now;
                //aynı isimde kategori var mı kontrol et
                var existingCategory = _categoryRepository.GetAll().FirstOrDefault(c => c.Name.ToLower() == categoryEntity.Name.ToLower());
                if (existingCategory != null)
                {
                    _zLogger.Error("Aynı isimde bir kategori zaten mevcut");
                    return ResponseGeneric<CategoryCreateDto>.Error("Aynı isimde bir kategori zaten mevcut");
                }
                _categoryRepository.Create(categoryEntity);
                _zLogger.Info($"Yeni kategori oluşturuldu: {categoryEntity.Name}");
                return ResponseGeneric<CategoryCreateDto>.Success(null, "kategori başarıyla oluşturuldu.");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Kategori oluşturulurken bir hata oluştu");
                return ResponseGeneric<CategoryCreateDto>.Error(ex.Message);
            }
            
            
        }

        //Delete çağrıldığı anda her koşulda başarılı diye mesaj dönüyoruz ama gerçekten başarılı mı?
        public IResponse<CategoryQuaryDto> Delete(int id)
        {
            try
            {            
            var category= _categoryRepository.GetByIdAsync(id).Result;

            if(category==null)
            {
                _zLogger.Error("kategori bulunamadı");
                    return ResponseGeneric<CategoryQuaryDto>.Error("kategori bulunamadı");
            }
            _categoryRepository.Delete(category);
                _zLogger.Info($"kategori silindi: {category.Name}");
                return ResponseGeneric<CategoryQuaryDto>.Success(null,"kategori başarıyla silindi");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Kategori silinirken bir hata oluştu");
                return ResponseGeneric<CategoryQuaryDto>.Error(ex.Message);
            }
        }

        public IResponse<CategoryQuaryDto> GetById(int id)
        {
            try
            {
            
            var category = _categoryRepository.GetByIdAsync(id).Result;
                var categoryDto = _mapper.Map<CategoryQuaryDto>(category);

                if ( categoryDto ==null)
            {
                _zLogger.Warn("kategori bulunamadı");
                    return ResponseGeneric<CategoryQuaryDto>.Success(null,"kategori bulunamadı");
            }
                _zLogger.Info($"kategori bulundu: {category.Name}");

                return ResponseGeneric<CategoryQuaryDto>.Success(categoryDto,"kategori bulundu.");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Kategori getirilirken bir hata oluştu");
                
                return ResponseGeneric<CategoryQuaryDto>.Error("Bir hata oluştu");
            }
        }

        public IResponse<IEnumerable<CategoryQuaryDto>> GetByName(string name)
        {
            try
            {
                var categories = _categoryRepository.GetAll().Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryQuaryDto>>(categories);

                if (categoryDtos == null || categories.Count == 0)
                {
                    _zLogger.Warn("kategori bulunamadı");
                    return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("kategori bulunamadı");
                }

                _zLogger.Info($"kategori bulundu: {name}");
                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Success(categoryDtos, "kategori Başarıyla bulundu");
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex,"Bir hata oluştu");
                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("Bir hata oluştu");
            }
            }


        public IResponse<IEnumerable<CategoryQuaryDto>> ListAll()
        {
            try
            { 
                var categories = _categoryRepository.GetAll().ToList();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryQuaryDto>>(categories);

            if( categoryDtos==null || categoryDtos.ToList().Count==0)
            {
                    _zLogger.Warn("kategori bulunamadı");
                    return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("kategori bulunamadı");
            }
                _zLogger.Info("Tüm kategoriler listelendi");

                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Success(categoryDtos, "kategoriler döndürüldü");

            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Bir hata oluştu");
                
                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("Bir hata oluştu.");
            }
           
        }

        public Task<IResponse<CategoryUpdateDto>> Update(CategoryUpdateDto categoryUpdateDto)
        {
            try
            { 
                // ilk önce dbden categoryi bulacağız
                var categoryEntity = _categoryRepository.GetByIdAsync(categoryUpdateDto.Id).Result;
                // category var mı?
                if (categoryEntity == null)
                {
                    _zLogger.Warn("kategori bulunamadı");
                    return Task.FromResult<IResponse<CategoryUpdateDto>>(ResponseGeneric<CategoryUpdateDto>.Error("kategori bulunamadı"));
                }

                //güncellenecek alanları kontrol edip öyle güncelle
                if (!string.IsNullOrEmpty(categoryUpdateDto.Name))
                {
                    categoryEntity.Name = categoryUpdateDto.Name;
                }
                if (!string.IsNullOrEmpty(categoryUpdateDto.Description))
                {
                    categoryEntity.Description = categoryUpdateDto.Description;
                }
                _categoryRepository.Update(categoryEntity);
                _zLogger.Info($"kategori güncellendi: {categoryEntity.Name}");
                return Task.FromResult<IResponse<CategoryUpdateDto>>(ResponseGeneric<CategoryUpdateDto>.Success(null, "Kitap başarıyla güncellendi"));
            }
            catch(Exception ex)
            {
                _zLogger.Error(ex, "Kategori güncellenirken bir hata oluştu");
                return Task.FromResult<IResponse<CategoryUpdateDto>>(ResponseGeneric<CategoryUpdateDto>.Error("Bir hata oluştu"));
            }
        }


    }
}
