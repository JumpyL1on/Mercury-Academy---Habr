using Habr.Common.Requests;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPostService
    {
        public Task CreateAsync(CreatePostRequest request, User user);
        public Task DeleteAsync(int id, User user);
    }
}