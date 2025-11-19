using KutuphaneCore.Entities;
using KutuphaneDataAccess;
using KutuphaneDataAccess.DTOs;
using Microsoft.EntityFrameworkCore;
using kutuphaneServis.Interfaces;
using kutuphaneServis.Response;
using kutuphaneServis.ResponseGeneric;
using System.Data;

namespace kutuphaneServis.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly DatabaseConnection _db;
        public BorrowService(DatabaseConnection db) => _db = db;

        // 1) Müsaitlik kontrolü (stok: Book.TotalCopies)
        public async Task<IResponse<AvailabilityDto>> CheckAvailabilityAsync(int bookId, DateOnly start, DateOnly end)
        {
            var book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId);
            if (book is null) return ResponseGeneric<AvailabilityDto>.Error("Kitap bulunamadı");

            var active = await _db.BorrowRequests.AsNoTracking()
                .Where(x => x.BookId == bookId &&
                            (x.Status == BorrowStatus.Approved || x.Status == BorrowStatus.CheckedOut) &&
                            start <= x.EndDate && end >= x.StartDate)
                .CountAsync();

            var availableCount = Math.Max(0, book.TotalCopies - active);
            var dto = new AvailabilityDto
            {
                Available = availableCount > 0,
                AvailableCount = availableCount,
                TotalCopies = book.TotalCopies
            };
            return ResponseGeneric<AvailabilityDto>.Success(dto, "Müsaitlik hesaplandı");
        }

        // 2) Kullanıcı isteği oluştur (Pending)
        public async Task<IResponse<int>> CreateRequestAsync(int userId, BorrowRequestCreateDto dto)
        {
            if (dto.EndDate < dto.StartDate) return ResponseGeneric<int>.Error("Bitiş tarihi başlangıçtan küçük olamaz");
            if (dto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date)) return ResponseGeneric<int>.Error("Geçmiş tarihe istek oluşturulamaz");

            var maxDays = 21;
            var span = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;
            if (span > maxDays) return ResponseGeneric<int>.Error($"Maksimum ödünç süresi {maxDays} gündür.");

            var exists = await _db.Books.AnyAsync(x => x.Id == dto.BookId);
            if (!exists) return ResponseGeneric<int>.Error("Kitap bulunamadı");

            var entity = new BorrowRequest
            {
                BookId = dto.BookId,
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = BorrowStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _db.BorrowRequests.Add(entity);
            await _db.SaveChangesAsync();

            return ResponseGeneric<int>.Success(entity.Id, "İstek oluşturuldu");
        }

        // 3) Admin onayı (yarışa dayanıklı)
        public async Task<IResponse<string>> ApproveAsync(int adminId, int requestId)
        {
            using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var req = await _db.BorrowRequests.Include(x => x.Book).FirstOrDefaultAsync(x => x.Id == requestId);
            if (req is null || req.Status != BorrowStatus.Pending)
                return ResponseGeneric<string>.Error("İstek bulunamadı ya da beklemiyor");

            var total = req.Book.TotalCopies;

            var active = await _db.BorrowRequests
                .Where(x => x.BookId == req.BookId &&
                            (x.Status == BorrowStatus.Approved || x.Status == BorrowStatus.CheckedOut) &&
                            req.StartDate <= x.EndDate && req.EndDate >= x.StartDate)
                .CountAsync();

            if (active >= total) return ResponseGeneric<string>.Error("Müsait kopya yok");

            req.Status = BorrowStatus.Approved;
            req.ApprovedBy = adminId;
            req.ApprovedAt = DateTime.UtcNow;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return ResponseGeneric<string>.Success(null, "İstek onaylandı");
        }

        public async Task<IResponse<string>> RejectAsync(int adminId, int requestId, string reason)
        {
            var req = await _db.BorrowRequests.FirstOrDefaultAsync(x => x.Id == requestId);
            if (req is null || req.Status != BorrowStatus.Pending)
                return ResponseGeneric<string>.Error("Sadece bekleyen istek reddedilebilir");

            req.Status = BorrowStatus.Rejected;
            req.RejectedReason = reason;
            req.ApprovedBy = adminId;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ResponseGeneric<string>.Success(null, "İstek reddedildi");
        }

        public async Task<IResponse<string>> CheckoutAsync(int adminId, int requestId)
        {
            var req = await _db.BorrowRequests.FirstOrDefaultAsync(x => x.Id == requestId);
            if (req is null || req.Status != BorrowStatus.Approved)
                return ResponseGeneric<string>.Error("Sadece onaylı kayıt teslim edilebilir");

            req.Status = BorrowStatus.CheckedOut;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ResponseGeneric<string>.Success(null, "Teslim edildi (checkout)");
        }

        public async Task<IResponse<string>> ReturnAsync(int adminId, int requestId)
        {
            var req = await _db.BorrowRequests.FirstOrDefaultAsync(x => x.Id == requestId);
            if (req is null || req.Status != BorrowStatus.CheckedOut)
                return ResponseGeneric<string>.Error("Sadece teslim edilmiş kayıt iade alınabilir");

            req.Status = BorrowStatus.Returned;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ResponseGeneric<string>.Success(null, "İade alındı (return)");
        }

        public async Task<IResponse<string>> CancelAsync(int userId, int requestId)
        {
            var req = await _db.BorrowRequests.FirstOrDefaultAsync(x => x.Id == requestId && x.UserId == userId);
            if (req is null) return ResponseGeneric<string>.Error("Kayıt bulunamadı");
            if (req.Status != BorrowStatus.Pending) return ResponseGeneric<string>.Error("Sadece bekleyen istek iptal edilebilir");

            req.Status = BorrowStatus.Cancelled;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ResponseGeneric<string>.Success(null, "İstek iptal edildi");
        }

        public async Task<IResponse<List<BorrowRequestListItemDto>>> MyRequestsAsync(int userId)
        {
            var list = await _db.BorrowRequests
                .Include(x => x.Book)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .Select(x => new BorrowRequestListItemDto
                {
                    Id = x.Id,
                    BookId = x.BookId,
                    BookTitle = x.Book.Title,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Status = x.Status.ToString()
                })
                .ToListAsync();

            return ResponseGeneric<List<BorrowRequestListItemDto>>.Success(list, "Ödünç/istekler getirildi");
        }

        public async Task<IResponse<List<BorrowRequestListItemDto>>> PendingQueueAsync()
        {
            var list = await _db.BorrowRequests
                .Include(x => x.Book)
                .Where(x => x.Status == BorrowStatus.Pending)
                .OrderBy(x => x.StartDate)
                .Select(x => new BorrowRequestListItemDto
                {
                    Id = x.Id,
                    BookId = x.BookId,
                    BookTitle = x.Book.Title,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Status = x.Status.ToString()
                })
                .ToListAsync();

            return ResponseGeneric<List<BorrowRequestListItemDto>>.Success(list, "Bekleyen istekler getirildi");
        }

        
        public async Task<IResponse<string>> ReturnByUserAsync(int userId, int requestId)
        {
            var req = await _db.BorrowRequests
                .FirstOrDefaultAsync(x => x.Id == requestId && x.UserId == userId);

            if (req == null)
                return ResponseGeneric<string>.Error("Kayıt bulunamadı.");

            if (req.Status != BorrowStatus.Approved)
                return ResponseGeneric<string>.Error("Sadece kullanıcıda olan (CheckedOut) kayıt iade edilebilir.");

            req.Status = BorrowStatus.Returned;
            req.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ResponseGeneric<string>.Success("Kitap iade edildi.");
        }

    }
}
