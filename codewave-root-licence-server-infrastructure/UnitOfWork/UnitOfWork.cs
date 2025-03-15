using codewave_root_licence_server_core.Interfaces;
using codewave_root_licence_server_infrastructure.Data;
using codewave_root_licence_server_infrastructure.Repositories;

namespace codewave_root_licence_server_infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly LicenseDbContext _dbContext;
    public ILicenseRepository Licenses { get; }

    public UnitOfWork(LicenseDbContext dbContext)
    {
        _dbContext = dbContext;
        Licenses = new LicenseRepository(dbContext);
    }

    public async Task<int> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}