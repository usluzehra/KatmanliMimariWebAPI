using AutoMapper;
using KutuphaneCore.DTOs;
using KutuphaneCore.Entities;
using KutuphaneDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.MapProfile
{
    public class MapProfile : Profile
    {

        public MapProfile()
        {
            // Burada DTO ile Entity arasında çift yönlü bir mapleme yapıyoruz.
            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorQuaryDto>().ReverseMap();

            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookQuaryDto>().ReverseMap();

            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryQuaryDto>().ReverseMap();
        }
    }
}
