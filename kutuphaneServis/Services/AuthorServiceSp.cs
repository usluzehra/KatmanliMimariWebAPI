using KutuphaneCore.DTOs;
using kutuphaneServis.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kutuphaneServis.ResponseGeneric;
using kutuphaneServis.Response;
using KutuphaneCore.Entities;

namespace kutuphaneServis.Services
{
    public class AuthorServiceSp : IAuthorServiceSP
    {
        private readonly string _connectionString;
        
        public AuthorServiceSp(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }


        private static AuthorQuaryDto MapQuaryDto(IDataRecord r) => new AuthorQuaryDto

        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            Name = r.GetString(r.GetOrdinal("Name")),
            Surname = r.GetString(r.GetOrdinal("Surname")),
            PlaceOfBirth = r.GetString(r.GetOrdinal("PlaceOfBirth")),
            YearOfBirth = r.GetInt32(r.GetOrdinal("YearOfBirth")),
            RecordDate = r.GetDateTime(r.GetOrdinal("RecordDate"))

        };

        private static AuthorUpdateDto MapUpdateDto(IDataRecord r) => new AuthorUpdateDto
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            Name = r.GetString(r.GetOrdinal("Name")),
            Surname = r.GetString(r.GetOrdinal("Surname")),
            PlaceOfBirth = r.GetString(r.GetOrdinal("PlaceOfBirth")),
            YearOfBirth = r.GetInt32(r.GetOrdinal("YearOfBirth"))
        };

        private static AuthorCreateDto MapCreateDto(IDataRecord r) => new AuthorCreateDto
        {
            Name = r.GetString(r.GetOrdinal("Name")),
            Surname = r.GetString(r.GetOrdinal("Surname")),
            PlaceOfBirth = r.GetString(r.GetOrdinal("PlaceOfBirth")),
            YearOfBirth = r.GetInt32(r.GetOrdinal("YearOfBirth"))
        };

        public IResponse<IEnumerable<AuthorQuaryDto>> ListAllSP()
        {
            var list = new List<AuthorQuaryDto>();
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsListAll", cn)
                { CommandType = CommandType.StoredProcedure };
                cn.Open();
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(MapQuaryDto(dr));
                }
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Success(list, "Yazarlar başarıyla listelendi");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error("Yazarlar bulunamadı.");
        }
        }


        public IResponse<AuthorQuaryDto> GetByIdSP(int id)
        {
            AuthorQuaryDto author = null;
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsGetById", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    author = MapQuaryDto(dr);
                }
                if (author == null)
                {
                    return ResponseGeneric<AuthorQuaryDto>.Error("Yazar bulunamadı.");
                }
                return ResponseGeneric<AuthorQuaryDto>.Success(author, "Yazar başarıyla bulundu.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<AuthorQuaryDto>.Error("Bir hata oluştu.");
            }
        }

        public IResponse<IEnumerable<AuthorQuaryDto>> GetByNameSP(string name)
        {
            var list = new List<AuthorQuaryDto>();
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsGetByName", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Name", name);
                cn.Open();
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(MapQuaryDto(dr));
                }
                if (list.Count == 0)
                {
                    return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error("Yazar bulunamadı.");
                }
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Success(list, "Yazarlar başarıyla bulundu.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<IEnumerable<AuthorQuaryDto>>.Error("Bir hata oluştu.");
            }
        }


        public IResponse<AuthorQuaryDto> DeleteSP(int id)
        {
            
            AuthorQuaryDto author = null;
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsDelete", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    author = MapQuaryDto(dr);
                }
                if (author == null)
                {
                    return ResponseGeneric<AuthorQuaryDto>.Error("Yazar bulunamadı.");
                }
                return ResponseGeneric<AuthorQuaryDto>.Success(author, "Yazar başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<AuthorQuaryDto>.Error("Bir hata oluştu.");
            }
        }

        public IResponse<AuthorCreateDto> CreateSP(AuthorCreateDto authorCreateDto)
        {
            //AuthorCreateDto author = null;
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsCreate", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Name", authorCreateDto.Name);
                cmd.Parameters.AddWithValue("@Surname", authorCreateDto.Surname);
                cmd.Parameters.AddWithValue("@PlaceOfBirth", authorCreateDto.PlaceOfBirth);
                cmd.Parameters.AddWithValue("@YearOfBirth", authorCreateDto.YearOfBirth);
                cn.Open();
                using var dr = cmd.ExecuteReader();

                if (authorCreateDto == null)
                {
                    return ResponseGeneric<AuthorCreateDto>.Error("Yazar oluşturulamadı.");
                }
                return ResponseGeneric<AuthorCreateDto>.Success(null, "Yazar başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<AuthorCreateDto>.Error("Bir hata oluştu.");
            }
        }

        public IResponse<AuthorUpdateDto> UpdateSP(AuthorUpdateDto authorUpdateDto)
        {
            AuthorUpdateDto author = null;
            try
            {
                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_AuthorsUpdate", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", authorUpdateDto.Id);
                cmd.Parameters.AddWithValue("@Name", authorUpdateDto.Name);
                cmd.Parameters.AddWithValue("@Surname", authorUpdateDto.Surname);
                cmd.Parameters.AddWithValue("@PlaceOfBirth", authorUpdateDto.PlaceOfBirth);
                cmd.Parameters.AddWithValue("@YearOfBirth", authorUpdateDto.YearOfBirth);
                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    author = MapUpdateDto(dr);
                }
                if (author == null)
                {
                    return ResponseGeneric<AuthorUpdateDto>.Error("Yazar bulunamadı.");
                }
                return ResponseGeneric<AuthorUpdateDto>.Success(author, "Yazar başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<AuthorUpdateDto>.Error("Bir hata oluştu.");
            }


        }

    }
}
