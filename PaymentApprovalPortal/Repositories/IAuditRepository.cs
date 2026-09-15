using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Repositories;

public interface IAuditRepository
{
    void Log(Guid paymentId, string description);
    IReadOnlyList<AuditEntry> GetByPaymentId(Guid paymentId);
}
