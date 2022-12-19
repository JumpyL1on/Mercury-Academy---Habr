using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntityConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> entityTypeBuilder)
        {
            entityTypeBuilder
                .Property(post => post.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entityTypeBuilder
                .Property(post => post.Title)
                .IsRequired()
                .HasMaxLength(200);

            entityTypeBuilder
                .Property(post => post.Text)
                .IsRequired()
                .HasMaxLength(2000);

            entityTypeBuilder
                .Property(post => post.CreatedAt)
                .IsRequired();

            entityTypeBuilder
                .Property(post => post.PublishedAt)
                .IsRequired(false);

            entityTypeBuilder
                .HasIndex(post => post.PublishedAt)
                .HasFilter("[PublishedAt] IS NOT NULL");

            entityTypeBuilder
                .Property(post => post.UpdatedAt)
                .IsRequired();

            entityTypeBuilder
                .HasIndex(post => post.UpdatedAt)
                .HasFilter(null);

            entityTypeBuilder
                .Property(post => post.AverageRating)
                .IsRequired(false);

            entityTypeBuilder
                .HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}