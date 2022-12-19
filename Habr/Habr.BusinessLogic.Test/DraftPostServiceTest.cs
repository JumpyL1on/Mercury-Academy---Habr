using AutoMapper;
using Habr.BusinessLogic.Services;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Habr.BusinessLogic.Test
{
    [TestFixture]
    public class DraftPostServiceTest
    {
        private DataContext _dbContext;
        private User user;
        private Mock<ILogger<DraftPostService>> _mockLogger;
        private DraftPostService _draftPostService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _dbContext = new DataContext(builder.Options);

            user = await CreateUserAsync();

            _mockLogger = new Mock<ILogger<DraftPostService>>();

            _draftPostService = CreateDraftPostService();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task PostPublication_PublishedPost_BusinessExceptionIsThrown()
        {
            // Arrange
            var publishedPostId = await CreatePublishedPostAsync();

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _draftPostService.PublishAsync(publishedPostId, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public void PostPublication_InvalidPostId_BusinessEceptionIsThrown()
        {
            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _draftPostService.PublishAsync(default, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public async Task PostPublication_InvalidUserId_AuthorizationEceptionIsThrown()
        {
            // Arrange
            var draftPostId = await CreateDraftPostAsync();

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _draftPostService.PublishAsync(draftPostId, user);
            };

            // Assert
            Assert.ThrowsAsync<ForbiddenException>(actual);
        }

        [Test]
        public async Task PostPublication_ValidDraftPost_Success()
        {
            // Arrange
            var draftPostId = await CreateDraftPostAsync();

            // Act
            await _draftPostService.PublishAsync(draftPostId, user);

            var actual = await _dbContext.Posts
                .AnyAsync(post => post.Id == draftPostId && post.PublishedAt != null);

            // Assert
            Assert.That(actual, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(_mockLogger.Invocations, Has.Count.EqualTo(1));

                Assert.That(_mockLogger.Invocations[0].Arguments[0], Is.EqualTo(LogLevel.Information));
            });
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

        private DraftPostService CreateDraftPostService()
        {
            var mockUserManager = new Mock<UserManager<User>>();

            var mockMapper = new Mock<IMapper>();

            var mockConfiguration = new Mock<IConfiguration>();

            return new DraftPostService(
                _dbContext,
                mockUserManager.Object,
                mockMapper.Object,
                _mockLogger.Object,
                mockConfiguration.Object);
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
    }
}