using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneDataAccess.DTOs
{
    // Kullanıcı tarih aralığına göre kitabın uygun olup olmadığını öğrenir
    public class AvailabilityDto
    {
        public bool Available { get; set; }
        public int AvailableCount { get; set; }
        public int TotalCopies { get; set; }
    }

    // Kullanıcının ödünç isteği oluştururken gönderdiği model
    public class BorrowRequestCreateDto
    {
        public int BookId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    // Listeleme (hem user hem admin tarafı)
    public class BorrowRequestListItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // Admin onay / reddet / teslim / iade işlemleri
    public class ApproveDto
    {
        public int Id { get; set; }
    }

    public class RejectDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CheckoutDto
    {
        public int Id { get; set; }
    }

    public class ReturnDto
    {
        public int Id { get; set; }
    }

    public class CancelDto
    {
        public int Id { get; set; }
    }
}