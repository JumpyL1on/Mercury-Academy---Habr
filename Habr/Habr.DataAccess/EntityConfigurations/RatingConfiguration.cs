using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntityConfigurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder
                .Property(rating => rating.Value)
                .IsRequired();

            builder
                .HasOne(rating => rating.Post)
                .WithMany(post => post.Ratings)
                .HasForeignKey(rating => rating.PostId);

            builder.HasKey(rating => new { rating.PostId, rating.UserId });

            builder
                .HasOne(rating => rating.User)
                .WithMany(user => user.Ratings)
                .HasForeignKey(rating => rating.UserId);
        }
    }
}