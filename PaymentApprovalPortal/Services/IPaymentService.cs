using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Services;

public interface IPaymentService
{
    IReadOnlyList<Payment> GetAll();
    Payment? GetById(Guid id);
    Payment Create(Payment payment);
    bool Approve(Guid id);
    bool Reject(Guid id);
    bool Delete(Guid id);
}
