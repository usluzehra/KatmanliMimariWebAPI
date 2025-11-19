using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneCore.DTOs
{
    public class AuthorCreateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PlaceOfBirth { get; set; }
        public int YearOfBirth { get; set; }
    }


    public class AuthorQuaryDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PlaceOfBirth { get; set; }
        public int YearOfBirth { get; set; }
        public string? ImageFileKey { get; set; }
    }

    public class AuthorUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PlaceOfBirth { get; set; }
        public int? YearOfBirth { get; set; }
    }



}
