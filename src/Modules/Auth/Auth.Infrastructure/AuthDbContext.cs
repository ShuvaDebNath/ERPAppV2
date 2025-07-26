using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure;
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<AspNetUser> AspNetUsers => Set<AspNetUser>();
    public DbSet<UserControl> UserControls => Set<UserControl>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<PagewiseAction> PagewiseActions => Set<PagewiseAction>();
    public DbSet<UserRole> Roles => Set<UserRole>();
    public DbSet<UserType> Types => Set<UserType>();
    public DbSet<UserDepartment> Departments => Set<UserDepartment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
