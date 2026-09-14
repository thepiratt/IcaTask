using PaymentApprovalPortal.Models;
using PaymentApprovalPortal.Repositories;

namespace PaymentApprovalPortal.Services;

public class PaymentService(IPaymentRepository repository) : IPaymentService
{
    public const decimal ApprovalThreshold = 10_000m;

    public Payment Create(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedDate = DateTime.Now;

        payment.Status = PaymentStatus.Draft;

        if (payment.Amount > ApprovalThreshold)
        {
            payment.Status = PaymentStatus.PendingApproval;
        }
        else
        {
            payment.Status = PaymentStatus.Executed;
        }

        repository.Add(payment);

        return payment;
    }

    public bool Delete(Guid id)
    {
        return repository.Delete(id);
    }

    public IReadOnlyList<Payment> GetAll()
    {
        return repository.GetAll();
    }

    public Payment? GetById(Guid id)
    {
        return repository.GetById(id);
    }

    public bool Approve(Guid id)
    {

        var payment = repository.GetById(id);

        if (payment is null || payment.Status != PaymentStatus.PendingApproval)
            return false;

        payment.Status = PaymentStatus.Approved;

        payment.Status = PaymentStatus.Executed;

        return repository.Update(payment);
    }

    public bool Reject(Guid id)
    {

        var payment = repository.GetById(id);

        if (payment is null || payment.Status != PaymentStatus.PendingApproval)
            return false;

        payment.Status = PaymentStatus.Rejected;

        return repository.Update(payment);
    }
}
