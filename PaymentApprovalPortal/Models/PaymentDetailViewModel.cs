namespace PaymentApprovalPortal.Models;

public class PaymentDetailViewModel
{
    public required Payment Payment { get; init; }
    public IReadOnlyList<AuditEntry> AuditEntries { get; init; } = Array.Empty<AuditEntry>();
}
