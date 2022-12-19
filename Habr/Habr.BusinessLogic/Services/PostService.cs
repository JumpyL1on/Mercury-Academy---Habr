using Habr.BusinessLogic.Interfaces;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly DbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PostService> _logger;

        public PostService(
            DbContext dbContext,
            UserManager<User> userManager,
            ILogger<PostService> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task CreateAsync(CreatePostRequest request, User user)
        {
            var createdAt = DateTime.UtcNow;

            var post = new Post
            {
                Title = request.Title,
                Text = request.Text,
                CreatedAt = createdAt,
                PublishedAt = request.SaveAsDraft ? null : createdAt,
                UpdatedAt = createdAt,
                UserId = user.Id
            };

            _dbContext
                .Set<Post>()
                .Add(post);

            await _dbContext.SaveChangesAsync();

            if (!request.SaveAsDraft)
            {
                _logger.LogInformation(LoggerMessageResource.PostWasPublished);
            }
        }

        public async Task DeleteAsync(int id, User user)
        {
            var posts = _dbContext.Set<Post>();

            var post = await posts.SingleOrDefaultAsync(post => post.Id == id);

            if (post == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
            }

            if (post.UserId != user.Id && await _userManager.IsInRoleAsync(user, RoleNameResource.User))
            {
                throw new ForbiddenException(ExceptionMessageResource.UserIsAbleToDeleteHisHerPostsOnly);
            }

            posts.Remove(post);

            await _dbContext.SaveChangesAsync();
        }
    }
}