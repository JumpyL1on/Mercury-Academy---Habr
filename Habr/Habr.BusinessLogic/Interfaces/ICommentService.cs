using Habr.Common.DTOs;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface ICommentService
    {
        public Task CreateAsync(int postId, CreateCommentRequest request, User user);
        public Task<List<CommentDTO>> GetAllByPostIdAsync(int postId);
        public Task DeleteAsync(int id, User user);
    }
}