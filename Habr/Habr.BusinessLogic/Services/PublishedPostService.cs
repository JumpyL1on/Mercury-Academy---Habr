using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.DTOs.V2;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using X.PagedList;

namespace Habr.BusinessLogic.Services
{
    public class PublishedPostService : IPublishedPostService
    {
        private readonly DbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly int maxPageSize;

        public PublishedPostService(
            DbContext dbContext,
            UserManager<User> userManager,
            ICommentService commentService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _commentService = commentService;
            _mapper = mapper;
            maxPageSize = int.Parse(configuration["MaxPageSize:PublishedPost"]);
        }

        public async Task<PaginatedDTO<PublishedPostDTO>> GetAllByPageAsync(PaginationQueryParameters parameters)
        {
            if (parameters.PageSize > maxPageSize)
            {
                var error = string.Format(ExceptionMessageResource.IncorrectPageSize, maxPageSize);

                throw new BusinessException(error);
            }

            var pagedList = await _dbContext
                .Set<Post>()
                .Where(post => post.PublishedAt != null)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(post => post.PublishedAt)
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return new PaginatedDTO<PublishedPostDTO>
            {
                PagedList = pagedList,
                TotalPagesCount = pagedList.PageCount,
                CurrentPage = pagedList.PageNumber,
                HasPreviousPage = pagedList.HasPreviousPage,
                HasNextPage = pagedList.HasNextPage
            };
        }

        public async Task<PaginatedDTO<PublishedPostDTOV2>> GetAllByPageAsyncV2(PaginationQueryParameters parameters)
        {
            if (parameters.PageSize > maxPageSize)
            {
                var error = string.Format(ExceptionMessageResource.IncorrectPageSize, maxPageSize);

                throw new BusinessException(error);
            }

            var pagedList = await _dbContext
                .Set<Post>()
                .Where(post => post.PublishedAt != null)
                .ProjectTo<PublishedPostDTOV2>(_mapper.ConfigurationProvider)
                .OrderByDescending(post => post.PublishedAt)
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return new PaginatedDTO<PublishedPostDTOV2>
            {
                PagedList = pagedList,
                TotalPagesCount = pagedList.PageCount,
                CurrentPage = pagedList.PageNumber,
                HasPreviousPage = pagedList.HasPreviousPage,
                HasNextPage = pagedList.HasNextPage
            };
        }

        public async Task<PublishedPostDetailsDTO> GetByIdAsync(int id)
        {
            var publishedPostDetails = await _dbContext
                .Set<Post>()
                .Where(post => post.Id == id)
                .ProjectTo<PublishedPostDetailsDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (publishedPostDetails == null)
            {
                throw new NotFoundException(ExceptionMessageResource.PostDoesNotExist);
            }

            if (publishedPostDetails.PublishedAt == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostIsNotPublished);
            }

            publishedPostDetails.Comments = await _commentService.GetAllByPostIdAsync(id);

            return publishedPostDetails;
        }

        public async Task MoveToDraftsAsync(int id, User user)
        {
            var post = await _dbContext
                .Set<Post>()
                .SingleOrDefaultAsync(post => post.Id == id);

            if (post == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
            }

            if (post.PublishedAt == null)
            {
                throw new BusinessException(ExceptionMessageResource.PostIsAlreadyDraft);
            }

            if (post.UserId != user.Id && await _userManager.IsInRoleAsync(user, RoleNameResource.User))
            {
                throw new ForbiddenException(ExceptionMessageResource.UserIsAbleToModifyHisHerPostsOnly);
            }

            if (await _dbContext
                .Set<Comment>()
                .AnyAsync(comment => comment.PostId == post.Id && !comment.IsDeleted))
            {
                throw new BusinessException(ExceptionMessageResource.ThereAreCommentsAttachedToPost);
            }

            post.PublishedAt = null;
            post.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}