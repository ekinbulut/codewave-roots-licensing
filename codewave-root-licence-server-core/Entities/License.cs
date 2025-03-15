namespace codewave_root_licence_server_core.Entities;

public class License
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AppName { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}