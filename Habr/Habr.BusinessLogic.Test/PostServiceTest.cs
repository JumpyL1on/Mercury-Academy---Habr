using Habr.BusinessLogic.Services;
using Habr.BusinessLogic.Validators;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Habr.BusinessLogic.Test
{
    [TestFixture]
    public class PostServiceTest
    {
        private DataContext _dbContext;
        private User user;
        private Mock<ILogger<PostService>> _mockLogger;
        private PostService _postService;
        private static string?[] TitleValues => new string?[] { null, "", new('t', 201) };
        private static string?[] TextValues => new string?[] { null, "", new('t', 2001) };

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _dbContext = new DataContext(builder.Options);

            user = await CreateUserAsync();

            _mockLogger = new Mock<ILogger<PostService>>();

            _postService = CreatePostService();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void PostCreation_InvalidRequest_ValidationIsFailed(
            [ValueSource(nameof(TitleValues))] string title,
            [ValueSource(nameof(TextValues))] string text)
        {
            // Arrange
            var request = new CreatePostRequest
            {
                SaveAsDraft = true,
                Title = title,
                Text = text,
            };

            var validator = new CreatePostRequestValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public async Task PostCreation_ValidRequest_Success()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                SaveAsDraft = true,
                Title = "title",
                Text = "text",
            };

            // Act
            await _postService.CreateAsync(request, user);

            var actual = await _dbContext.Posts
                .AnyAsync(post => post.Title == request.Title && post.PublishedAt == null);

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public async Task PostPublication_ValidRequest_Success()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                SaveAsDraft = false,
                Title = "uniqueTitle",
                Text = "text",
            };

            // Act
            await _postService.CreateAsync(request, user);

            var actual = await _dbContext.Posts
                .AnyAsync(post => post.Title == request.Title && post.PublishedAt != null);

            // Assert
            Assert.That(actual, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(_mockLogger.Invocations, Has.Count.EqualTo(1));

                Assert.That(_mockLogger.Invocations[0].Arguments[0], Is.EqualTo(LogLevel.Information));
            });
        }

        [Test]
        public void PostDeletion_InvalidPostId_BusinessExceptionIsThrown()
        {
            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _postService.DeleteAsync(default, user);
            };

            // Assert
            Assert.ThrowsAsync<BusinessException>(actual);
        }

        [Test]
        public async Task PostDeletion_InvalidUserId_AuthorizationExceptionIsThrown()
        {
            // Arrange
            var postId = await CreatePostAsync();

            // Act
            AsyncTestDelegate actual = async () =>
            {
                await _postService.DeleteAsync(postId, default);
            };

            // Assert
            Assert.ThrowsAsync<ForbiddenException>(actual);
        }

        [Test]
        public async Task PostDeletion_ValidPost_Success()
        {
            // Arrange
            var postId = await CreatePostAsync();

            // Act
            await _postService.DeleteAsync(postId, user);

            var actual = await _dbContext.Posts
                .AnyAsync(post => post.Id == postId);

            // Assert
            Assert.That(actual, Is.False);
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

        private PostService CreatePostService()
        {
            var mockUserManager = new Mock<UserManager<User>>();

            return new PostService(_dbContext, mockUserManager.Object, _mockLogger.Object);
        }

        private async Task<int> CreatePostAsync()
        {
            var createdAt = DateTime.UtcNow;

            var post = new Post
            {
                Title = "title",
                Text = "text",
                CreatedAt = createdAt,
                PublishedAt = createdAt,
                UpdatedAt = createdAt,
                UserId = user.Id
            };

            _dbContext.Posts.Add(post);

            await _dbContext.SaveChangesAsync();

            return post.Id;
        }
    }
}