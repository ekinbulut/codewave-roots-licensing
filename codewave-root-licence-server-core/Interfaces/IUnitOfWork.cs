namespace codewave_root_licence_server_core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILicenseRepository Licenses { get; }
    Task<int> CompleteAsync();
}