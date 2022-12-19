using Habr.BusinessLogic.Interfaces;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services
{
    public class RatingService : IRatingService
    {
        private readonly DbContext _dbContext;

        public RatingService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(int postId, CreateRatingRequest request, User user)
        {
            var ratings = _dbContext.Set<Rating>();

            var rating = await ratings
                .SingleOrDefaultAsync(rating => rating.PostId == postId && rating.UserId == user.Id);

            if (rating == null)
            {
                var post = await _dbContext
                    .Set<Post>()
                    .SingleOrDefaultAsync(post => post.Id == postId);

                if (post == null)
                {
                    throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
                }

                if (post.PublishedAt == null)
                {
                    throw new BusinessException(ExceptionMessageResource.PostIsNotPublished);
                }

                rating = new Rating
                {
                    Value = request.Value,
                    PostId = postId,
                    UserId = user.Id
                };

                ratings.Add(rating);
            }
            else
            {
                rating.Value = request.Value;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAverageRatingForAllPublishedPostsAsync()
        {
            var dictionary = await _dbContext
                .Set<Rating>()
                .GroupBy(rating => rating.PostId)
                .Select(grouping => new
                {
                    Id = grouping.Key,
                    AverageRating = grouping.Average(rating => rating.Value)
                })
                .ToDictionaryAsync(x => x.Id, x => Math.Round(x.AverageRating, 2));

            await _dbContext
                .Set<Post>()
                .Where(post => dictionary.Keys.Contains(post.Id))
                .ForEachAsync(post => post.AverageRating = dictionary[post.Id]);

            await _dbContext.SaveChangesAsync();
        }
    }
}