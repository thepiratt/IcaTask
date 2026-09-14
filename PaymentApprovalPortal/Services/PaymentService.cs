using PaymentApprovalPortal.Models;
using PaymentApprovalPortal.Repositories;

namespace PaymentApprovalPortal.Services;

public class PaymentService(IPaymentRepository repository, IConfiguration configuration) : IPaymentService
{
    private readonly decimal approvalThreshold = configuration.GetValue<decimal>("PaymentSettings:ApprovalThreshold", 10_000m);

    public Payment Create(Payment payment)
    {
        payment.MarkCreated();

        if (payment.Amount > approvalThreshold)
        {
            payment.MarkPendingApproval();
        }
        else
        {
            payment.Execute();
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

        payment.Approve();
        payment.Execute();

        return repository.Update(payment);
    }

    public bool Reject(Guid id)
    {

        var payment = repository.GetById(id);

        if (payment is null || payment.Status != PaymentStatus.PendingApproval)
            return false;

        payment.Reject();

        return repository.Update(payment);
    }
}
