using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.DTOs.V2;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPublishedPostService
    {
        public Task<PaginatedDTO<PublishedPostDTO>> GetAllByPageAsync(PaginationQueryParameters parameters);
        public Task<PaginatedDTO<PublishedPostDTOV2>> GetAllByPageAsyncV2(PaginationQueryParameters parameters);
        public Task<PublishedPostDetailsDTO> GetByIdAsync(int id);
        public Task MoveToDraftsAsync(int id, User user);
    }
}