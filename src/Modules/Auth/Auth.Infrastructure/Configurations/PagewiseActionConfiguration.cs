using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Configurations;

public class PagewiseActionConfiguration : IEntityTypeConfiguration<PagewiseAction>
{
    public void Configure(EntityTypeBuilder<PagewiseAction> builder)
    {
        builder.ToTable("tblPagewiseAction");
        builder.HasKey(p => p.ActionID);
    }
}
