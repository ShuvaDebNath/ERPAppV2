using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Configurations;

public class UserDepartmentConfiguration : IEntityTypeConfiguration<UserDepartment>
{
    public void Configure(EntityTypeBuilder<UserDepartment> builder)
    {
        builder.ToTable("tblUserDepartment");
        builder.HasKey(d => d.DeptId);
        builder.Ignore(d => d.Users);
    }
}
