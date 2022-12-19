using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Habr.BusinessLogic.Test
{
    [TestFixture]
    public class PublishedPostServiceTest
    {
        private DataContext _dbContext;
        private User user;
        private PublishedPostService _publishedPostService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _dbContext = new DataContext(builder.Options);

            user = await CreateUserAsync();

            _publishedPostService = CreatePublishedPostService();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task MovingPostToDraft_DraftPost_BusinessExceptionIsThrown()
        {
            // Arrange
            var draftPostId = await CreateDraftPostAsync();

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _publishedPostService.MoveToDraftsAsync(draftPostId, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public void MovingPostToDraft_InvalidPostId_BusinessEceptionIsThrown()
        {
            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _publishedPostService.MoveToDraftsAsync(default, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public async Task MovingPostToDraft_InvalidUserId_AuthorizationEceptionIsThrown()
        {
            // Arrange
            var publishedPostId = await CreatePublishedPostAsync();

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _publishedPostService.MoveToDraftsAsync(publishedPostId, user);
            };

            // Assert
            Assert.ThrowsAsync<ForbiddenException>(actual);
        }

        [Test]
        public async Task MovingPostToDraft_CommentIsAttachedToPost_BusinessEceptionIsThrown()
        {
            // Arrange
            var publishedPostId = await CreatePublishedPostAsync();

            await CreateCommentAsync(publishedPostId);

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _publishedPostService.MoveToDraftsAsync(publishedPostId, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public async Task MovingPostToDraft_ValidPublishedPost_Success()
        {
            // Arrange
            var publishedPostId = await CreatePublishedPostAsync();

            // Act
            await _publishedPostService.MoveToDraftsAsync(publishedPostId, user);

            var actual = await _dbContext.Posts
                .AnyAsync(post => post.Id == publishedPostId && post.PublishedAt == null);

            // Assert
            Assert.That(actual, Is.True);
        }

        private async Task<User> CreateUserAsync()
        {
            var user = new User
            {
                UserName = "name",
                Email = "email",
                PasswordHash = "hash"
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return user;
        }

        private PublishedPostService CreatePublishedPostService()
        {
            var mockUserManager = new Mock<UserManager<User>>();

            var mockCommentService = new Mock<ICommentService>();

            var mockMapper = new Mock<IMapper>();

            var mockConfiguration = new Mock<IConfiguration>();

            return new PublishedPostService(
                _dbContext,
                mockUserManager.Object,
                mockCommentService.Object,
                mockMapper.Object,
                mockConfiguration.Object);
        }

        private async Task<int> CreateDraftPostAsync()
        {
            var createdAt = DateTime.UtcNow;

            var draftPost = new Post
            {
                Title = "title",
                Text = "text",
                CreatedAt = createdAt,
                PublishedAt = null,
                UpdatedAt = createdAt,
                UserId = user.Id
            };

            _dbContext.Posts.Add(draftPost);

            await _dbContext.SaveChangesAsync();

            return draftPost.Id;
        }

        private async Task<int> CreatePublishedPostAsync()
        {
            var createdAt = DateTime.UtcNow;

            var publishedPost = new Post
            {
                Title = "title",
                Text = "text",
                CreatedAt = createdAt,
                PublishedAt = createdAt,
                UpdatedAt = createdAt,
                UserId = user.Id
            };

            _dbContext.Posts.Add(publishedPost);

            await _dbContext.SaveChangesAsync();

            return publishedPost.Id;
        }

        private async Task CreateCommentAsync(int postId)
        {
            var comment = new Comment
            {
                Text = "text",
                CreatedAt = DateTime.UtcNow,
                PostId = postId,
                UserId = user.Id,
                ParentId = null
            };

            _dbContext.Comments.Add(comment);

            await _dbContext.SaveChangesAsync();
        }
    }
}