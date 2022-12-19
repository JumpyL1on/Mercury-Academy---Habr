using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder
                .Property(user => user.Name)
                .IsRequired(false)
                .HasMaxLength(200);

            entityTypeBuilder
                .Property(user => user.RefreshToken)
                .IsRequired(false)
                .HasMaxLength(88);

            entityTypeBuilder
                .Property(user => user.RefreshTokenExpiryTime)
                .IsRequired(false);

            entityTypeBuilder
                .HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            entityTypeBuilder
                .HasMany(user => user.Comments)
                .WithOne(comment => comment.User)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}