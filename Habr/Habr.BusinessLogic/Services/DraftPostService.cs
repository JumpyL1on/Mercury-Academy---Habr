using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace Habr.BusinessLogic.Services
{
    public class DraftPostService : IDraftPostService
    {
        private readonly DbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<DraftPostService> _logger;
        private readonly int maxPageSize;

        public DraftPostService(
            DbContext dbContext,
            UserManager<User> userManager,
            IMapper mapper,
            ILogger<DraftPostService> logger,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            maxPageSize = int.Parse(configuration["MaxPageSize:DraftPost"]);
        }

        public async Task<PaginatedDTO<DraftPostDTO>> GetAllByUserAndPageAsync(
            User user,
            PaginationQueryParameters parameters)
        {
            if (parameters.PageSize > maxPageSize)
            {
                var error = string.Format(ExceptionMessageResource.IncorrectPageSize, maxPageSize);

                throw new BusinessException(error);
            }

            var pagedList = await _dbContext
                .Set<Post>()
                .Where(post => post.UserId == user.Id && post.PublishedAt == null)
                .ProjectTo<DraftPostDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(post => post.UpdatedAt)
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return new PaginatedDTO<DraftPostDTO>
            {
                PagedList = pagedList,
                TotalPagesCount = pagedList.PageCount,
                CurrentPage = pagedList.PageNumber,
                HasPreviousPage = pagedList.HasPreviousPage,
                HasNextPage = pagedList.HasNextPage
            };
        }

        public async Task EditAsync(int id, EditDraftPostRequest request, User user)
        {
            var post = await _dbContext
                .Set<Post>()
                .SingleOrDefaultAsync(post => post.Id == id);

            if (post == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
            }

            if (post.PublishedAt != null)
            {
                throw new BusinessException(ExceptionMessageResource.PublishedPostCannotBeEdited);
            }

            if (post.UserId != user.Id && await _userManager.IsInRoleAsync(user, RoleNameResource.User))
            {
                throw new ForbiddenException(ExceptionMessageResource.UserIsAbleToModifyHisHerPostsOnly);
            }
                
            post.Title = request.NewTitle;
            post.Text = request.NewText;
            post.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task PublishAsync(int id, User user)
        {
            var post = await _dbContext
                .Set<Post>()
                .SingleOrDefaultAsync(post => post.Id == id);

            if (post == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
            }

            if (post.PublishedAt != null)
            {
                throw new BusinessException(ExceptionMessageResource.PostIsAlreadyPublished);
            }

            if (post.UserId != user.Id && await _userManager.IsInRoleAsync(user, RoleNameResource.User))
            {
                throw new ForbiddenException(ExceptionMessageResource.UserIsAbleToModifyHisHerPostsOnly);
            }

            post.PublishedAt = post.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(LoggerMessageResource.PostWasPublished);
        }
    }
}