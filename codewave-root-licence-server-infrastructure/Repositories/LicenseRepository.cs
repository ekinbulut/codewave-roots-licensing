using codewave_root_licence_server_core.Entities;
using codewave_root_licence_server_core.Interfaces;
using codewave_root_licence_server_infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace codewave_root_licence_server_infrastructure.Repositories;

public class LicenseRepository : ILicenseRepository
{
    private readonly LicenseDbContext _dbContext;

    public LicenseRepository(LicenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<License> AddLicenseAsync(License license)
    {
        _dbContext.Licenses.Add(license);
        return license;
    }

    public async Task<License?> GetLicenseByKeyAsync(string licenseKey)
    {
        return await _dbContext.Licenses.FirstOrDefaultAsync(l => l.LicenseKey == licenseKey);
    }
}