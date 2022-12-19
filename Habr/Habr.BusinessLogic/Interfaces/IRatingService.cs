using Habr.Common.Requests;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IRatingService
    {
        public Task CreateAsync(int postId, CreateRatingRequest request, User user);
        public Task UpdateAverageRatingForAllPublishedPostsAsync();
    }
}