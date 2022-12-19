using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services
{
    public class CommentService : ICommentService
    {
        private readonly DbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CommentService(DbContext dbContext, UserManager<User> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<CommentDTO>> GetAllByPostIdAsync(int postId)
        {
            var preliminaryComments = await _dbContext
                .Set<Comment>()
                .Where(comment => comment.PostId == postId && !comment.IsDeleted)
                .ProjectTo<PreliminaryCommentDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(preliminaryComment => preliminaryComment.CreatedAt)
                .ToListAsync();

            var preliminaryCommentsLookup = preliminaryComments
                .ToLookup(preliminaryComment => preliminaryComment.ParentId);

            var comments = new List<CommentDTO>();

            foreach (var parent in preliminaryCommentsLookup[null])
            {
                comments.Add(new CommentDTO
                {
                    Text = parent.Text,
                    CreatedAt = parent.CreatedAt,
                    AuthorEmail = parent.AuthorEmail,
                    Childrens = GetChildrens(parent.Id, preliminaryCommentsLookup)
                });
            }

            return comments;
        }

        public async Task CreateAsync(int postId, CreateCommentRequest request, User user)
        {
            if (!await _dbContext
                .Set<Post>()
                .AnyAsync(post => post.Id == postId))
            {
                throw new BusinessException(ExceptionMessageResource.PostDoesNotExist);
            }

            var comments = _dbContext.Set<Comment>();

            if (request.ParentId != null && !await comments.AnyAsync(comment => comment.Id == request.ParentId))
            {
                throw new BusinessException(ExceptionMessageResource.CommentDoesNotExist);
            }

            var comment = new Comment
            {
                Text = request.Text,
                CreatedAt = DateTime.UtcNow,
                PostId = postId,
                UserId = user.Id,
                ParentId = request.ParentId
            };

            comments.Add(comment);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, User user)
        {
            var comment = await _dbContext
                .Set<Comment>()
                .SingleOrDefaultAsync(comment => comment.Id == id);

            if (comment == null)
            {
                throw new BusinessException(ExceptionMessageResource.CommentIsNotFound);
            }

            if (comment.UserId != user.Id && await _userManager.IsInRoleAsync(user, RoleNameResource.User))
            {
                throw new ForbiddenException(ExceptionMessageResource.UserCannotDeleteCommentsOfOtherUsers);
            }

            comment.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }

        private List<CommentDTO> GetChildrens(int parentId, ILookup<int?, PreliminaryCommentDTO> preliminaryCommentsLookup)
        {
            var childrens = new List<CommentDTO>();

            foreach (var children in preliminaryCommentsLookup[parentId])
            {
                childrens.Add(new CommentDTO
                {
                    Text = children.Text,
                    CreatedAt = children.CreatedAt,
                    AuthorEmail = children.AuthorEmail,
                    Childrens = GetChildrens(children.Id, preliminaryCommentsLookup)
                });
            }

            return childrens;
        }
    }
}