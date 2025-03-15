using System.Security.Cryptography;
using codewave_root_licence_server_core.Entities;
using codewave_root_licence_server_core.Interfaces;

namespace codewave_root_licence_server_core.Services;

public class LicenseService
{
    private readonly IUnitOfWork _unitOfWork;

    public LicenseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<License> GenerateLicenseAsync(string appName, DateTime expiryDate)
    {
        var license = new License
        {
            AppName = appName,
            LicenseKey = GenerateSecureLicenseKey(),
            ExpiryDate = expiryDate
        };

        await _unitOfWork.Licenses.AddLicenseAsync(license);
        await _unitOfWork.CompleteAsync();
        return license;
    }

    public async Task<bool> ValidateLicenseAsync(string licenseKey)
    {
        var license = await _unitOfWork.Licenses.GetLicenseByKeyAsync(licenseKey);
        return license is not null && license.IsActive && license.ExpiryDate > DateTime.UtcNow;
    }

    public async Task<bool> RevokeLicenseAsync(string licenseKey)
    {
        var license = await _unitOfWork.Licenses.GetLicenseByKeyAsync(licenseKey);
        if (license is null) return false;

        license.IsActive = false;
        await _unitOfWork.CompleteAsync();
        return true;
    }

    private static string GenerateSecureLicenseKey()
    {
        using var hmac = new HMACSHA256();
        var bytes = hmac.ComputeHash(Guid.NewGuid().ToByteArray());
        return Convert.ToBase64String(bytes).Replace("=", "").Replace("+", "").Replace("/", "");
    }
}