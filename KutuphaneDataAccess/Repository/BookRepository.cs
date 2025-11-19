using KutuphaneDataAccess.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneDataAccess.Repository
{
    public sealed class BookRepository(IConfiguration configuration) : IBookRepository
    {
        private readonly string _cs = configuration.GetConnectionString("DefaultConnection")!;

        public async Task<List<BookCandidate>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<BookCandidate>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(
                "SELECT Id, Title, Description, CountOfPage, AuthorId, CategoryId FROM Books", con);

            await con.OpenAsync(ct);
            using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
            {
                list.Add(new BookCandidate {
                  Id=  rdr.GetInt32(0),
                  Title= rdr.GetString(1),
                  Description=  rdr.IsDBNull(2) ? null : rdr.GetString(2),
                  CountOfPage= rdr.GetInt32(3),
                  AuthorId= rdr.GetInt32(4),
                  CategoryId= rdr.GetInt32(5)
                });
            }
            return list;
        }
    }

}
