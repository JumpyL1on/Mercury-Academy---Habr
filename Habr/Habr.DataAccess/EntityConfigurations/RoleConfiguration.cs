using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntityConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = RoleNameResource.User,
                    NormalizedName = RoleNameResource.User.ToUpperInvariant(),
                },
                new Role
                {
                    Id = 2,
                    Name = RoleNameResource.Admin,
                    NormalizedName = RoleNameResource.Admin.ToUpperInvariant(),
                }
            };

            builder.HasData(roles);
        }
    }
}