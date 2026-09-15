using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Services;

public interface IAuditService
{
    void Log(Guid paymentId, string description);
    IReadOnlyList<AuditEntry> GetByPaymentId(Guid paymentId);
}
