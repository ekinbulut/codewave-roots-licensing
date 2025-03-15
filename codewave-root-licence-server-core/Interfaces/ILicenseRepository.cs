using codewave_root_licence_server_core.Entities;

namespace codewave_root_licence_server_core.Interfaces;

public interface ILicenseRepository
{
    Task<License> AddLicenseAsync(License license);
    Task<License?> GetLicenseByKeyAsync(string licenseKey);
}