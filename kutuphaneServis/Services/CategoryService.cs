using AutoMapper;
using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using KutuphaneDataAccess.Repository;
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


        public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<IResponse<CategoryCreateDto>> Create(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                if (categoryCreateDto == null)
                {
                    return ResponseGeneric<CategoryCreateDto>.Error("kategori bilgileri boş olamaz");
                }

                //DTOyu entitye maple
                var categoryEntity = new Category{ Name = categoryCreateDto.Name,Description = categoryCreateDto.Description };
                categoryEntity.RecordDate = DateTime.Now;

                _categoryRepository.Create(categoryEntity);
                return ResponseGeneric<CategoryCreateDto>.Success(null, "kategori başarıyla oluşturuldu.");
            }
            catch
            {
                return ResponseGeneric<CategoryCreateDto>.Error("Bir hata oluştu");
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
                return ResponseGeneric<CategoryQuaryDto>.Error("kategori bulunamadı");
            }
            _categoryRepository.Delete(category);
            return ResponseGeneric<CategoryQuaryDto>.Success(null,"kategori başarıyla silindi");
            }
            catch
            {
                return ResponseGeneric<CategoryQuaryDto>.Error("Bir hata oluştu.");
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
                return ResponseGeneric<CategoryQuaryDto>.Success(null,"kategori bulunamadı");
            }

            return ResponseGeneric<CategoryQuaryDto>.Success(categoryDto,"kategori bulundu.");
            }
            catch
            {
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
                    return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("kategori bulunamadı");
                }

                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Success(categoryDtos, "kategori Başarıyla bulundu");
            }
            catch
            {
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
                return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Error("kategori bulunamadı");
            }

            return ResponseGeneric<IEnumerable<CategoryQuaryDto>>.Success(categoryDtos, "kategoriler döndürüldü");

            }
            catch
            {
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
                return Task.FromResult<IResponse<CategoryUpdateDto>>(ResponseGeneric<CategoryUpdateDto>.Success(null, "Kitap başarıyla güncellendi"));
            }
            catch
            {
                return Task.FromResult<IResponse<CategoryUpdateDto>>(ResponseGeneric<CategoryUpdateDto>.Error("Bir hata oluştu"));
            }
        }


    }
}
