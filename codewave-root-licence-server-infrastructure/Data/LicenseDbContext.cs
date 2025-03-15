using codewave_root_licence_server_core.Entities;
using Microsoft.EntityFrameworkCore;

namespace codewave_root_licence_server_infrastructure.Data;

public class LicenseDbContext : DbContext
{
    public LicenseDbContext(DbContextOptions<LicenseDbContext> options) : base(options) { }
    public DbSet<License> Licenses { get; set; }
}