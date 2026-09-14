using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Repositories;

public interface IPaymentRepository
{
    IReadOnlyList<Payment> GetAll();
    Payment? GetById(Guid id);
    void Add(Payment payment);
    bool Update(Payment payment);
    bool Delete(Guid id);
}
