using KutuphaneDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KutuphaneDataAccess.Repository
{
    public interface IBookRepository
    {
        Task<List<BookCandidate>> GetAllAsync(CancellationToken ct = default);
    }
}
