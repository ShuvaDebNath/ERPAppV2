using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserControlConfiguration : IEntityTypeConfiguration<UserControl>
{
    public void Configure(EntityTypeBuilder<UserControl> builder)
    {
        builder.ToTable("tblUserControl");
        builder.HasKey(u => u.UserId);
        builder.Ignore(u => u.Department);
    }
}
