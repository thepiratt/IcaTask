using PaymentApprovalPortal.Models;
using System.Collections.Concurrent;

namespace PaymentApprovalPortal.Services;

public class AuditService : IAuditService
{
    private readonly ConcurrentDictionary<Guid, List<AuditEntry>> auditEntries = new();

    public void Log(Guid paymentId, string description)
    {
        var entries = auditEntries.GetOrAdd(paymentId, _ => new List<AuditEntry>());
        entries.Add(new AuditEntry(description));
    }
    public IReadOnlyList<AuditEntry> GetByPaymentId(Guid paymentId)
    {
        return auditEntries.TryGetValue(paymentId, out var entries)
            ? entries.AsReadOnly()
            : Array.Empty<AuditEntry>();
    }
}
