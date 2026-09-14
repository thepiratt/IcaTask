namespace PaymentApprovalPortal.Models;

public class AuditEntry
{
    public DateTime Timestamp { get; }
    public string Description { get; }

    public AuditEntry(string description)
    {
        Timestamp = DateTime.UtcNow;
        Description = description;
    }
}
