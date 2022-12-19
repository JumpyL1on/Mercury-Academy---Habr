using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntityConfigurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> entityTypeBuilder)
        {
            entityTypeBuilder
                .Property(comment => comment.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entityTypeBuilder
                .Property(comment => comment.Text)
                .IsRequired();

            entityTypeBuilder
                .Property(comment => comment.CreatedAt)
                .IsRequired();

            entityTypeBuilder
                .HasIndex(comment => comment.CreatedAt)
                .HasFilter(null);

            entityTypeBuilder
                .Property(comment => comment.IsDeleted)
                .IsRequired();

            entityTypeBuilder
                .HasMany(comment => comment.Childrens)
                .WithOne(comment => comment.Parent)
                .HasForeignKey(comment => comment.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);
        }
    }
}