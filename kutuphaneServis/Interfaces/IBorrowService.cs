using KutuphaneDataAccess.DTOs;
using kutuphaneServis.Response;
using KutuphaneCore.Entities;

namespace kutuphaneServis.Interfaces
{
    public interface IBorrowService
    {
        Task<IResponse<AvailabilityDto>> CheckAvailabilityAsync(int bookId, DateOnly start, DateOnly end);
        Task<IResponse<int>> CreateRequestAsync(int userId, BorrowRequestCreateDto dto);

        Task<IResponse<string>> ApproveAsync(int adminId, int requestId);
        Task<IResponse<string>> RejectAsync(int adminId, int requestId, string reason);

        Task<IResponse<string>> CheckoutAsync(int adminId, int requestId);
        Task<IResponse<string>> ReturnAsync(int adminId, int requestId);
       
        Task<IResponse<string>> ReturnByUserAsync(int userId, int requestId);


        Task<IResponse<string>> CancelAsync(int userId, int requestId);

        Task<IResponse<List<BorrowRequestListItemDto>>> MyRequestsAsync(int userId);
        Task<IResponse<List<BorrowRequestListItemDto>>> PendingQueueAsync();
    }
}
