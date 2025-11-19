using System.Threading.Tasks;
using kutuphaneServis.ResponseGeneric;
using Microsoft.AspNetCore.Http;
using KutuphaneDataAccess.DTOs;

namespace kutuphaneServis.Interfaces
{
    public interface IUploadImageService
    {
        Task<IResponse<UploadFileResponseDto>> UploadFile(IFormFile file, string entity, int entityId);
        Task<IResponse<ShowFileDto>> ShowFile(string fileKey);
    }
}
