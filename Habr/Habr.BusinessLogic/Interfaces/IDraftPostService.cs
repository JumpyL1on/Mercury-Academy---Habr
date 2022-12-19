using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IDraftPostService
    {
        public Task<PaginatedDTO<DraftPostDTO>> GetAllByUserAndPageAsync(User user, PaginationQueryParameters parameters);
        public Task EditAsync(int id, EditDraftPostRequest request, User user);
        public Task PublishAsync(int id, User user);
    }
}