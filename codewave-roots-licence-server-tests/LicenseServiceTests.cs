using codewave_root_licence_server_core.Entities;
using codewave_root_licence_server_core.Interfaces;
using codewave_root_licence_server_core.Services;
using Moq;

namespace codewave_roots_licence_server_tests;

public class LicenseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILicenseRepository> _licenseRepositoryMock;
    private readonly LicenseService _sut;

    public LicenseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _licenseRepositoryMock = new Mock<ILicenseRepository>();
        _unitOfWorkMock.Setup(x => x.Licenses).Returns(_licenseRepositoryMock.Object);
        _sut = new LicenseService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GenerateLicenseAsync_CreatesValidLicense()
    {
        var appName = "TestApp";
        var expiryDate = DateTime.UtcNow.AddDays(30);

        var result = await _sut.GenerateLicenseAsync(appName, expiryDate);

        Assert.Equal(appName, result.AppName);
        Assert.Equal(expiryDate, result.ExpiryDate);
        Assert.NotEmpty(result.LicenseKey);
        _licenseRepositoryMock.Verify(x => x.AddLicenseAsync(It.IsAny<License>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateLicenseAsync_WithValidActiveLicense_ReturnsTrue()
    {
        var license = new License
        {
            IsActive = true,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync(license);

        var result = await _sut.ValidateLicenseAsync("test-key");

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateLicenseAsync_WithExpiredLicense_ReturnsFalse()
    {
        var license = new License
        {
            IsActive = true,
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync(license);

        var result = await _sut.ValidateLicenseAsync("test-key");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateLicenseAsync_WithInactiveLicense_ReturnsFalse()
    {
        var license = new License
        {
            IsActive = false,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync(license);

        var result = await _sut.ValidateLicenseAsync("test-key");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateLicenseAsync_WithNonExistentLicense_ReturnsFalse()
    {
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((License)null);

        var result = await _sut.ValidateLicenseAsync("test-key");

        Assert.False(result);
    }

    [Fact]
    public async Task RevokeLicenseAsync_WithExistingLicense_RevokesAndReturnsTrue()
    {
        var license = new License { IsActive = true };
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync(license);

        var result = await _sut.RevokeLicenseAsync("test-key");

        Assert.True(result);
        Assert.False(license.IsActive);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RevokeLicenseAsync_WithNonExistentLicense_ReturnsFalse()
    {
        _licenseRepositoryMock.Setup(x => x.GetLicenseByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((License)null);

        var result = await _sut.RevokeLicenseAsync("test-key");

        Assert.False(result);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
    }
}