using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Services;

public class PaymentService : IPaymentService
{
    public const decimal ApprovalThreshold = 10_000m;

    private readonly List<Payment> _payments = new();
    private readonly object _lock = new();

    public bool Approve(Guid id)
    {
        lock (_lock)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);

            if (payment is null || payment.Status != PaymentStatus.PendingApproval)
            {
                return false;
            }

            payment.Status = PaymentStatus.Approved;

            payment.Status = PaymentStatus.Executed;

            return true;
        }
    }

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

        lock (_lock)
        {
            _payments.Add(payment);
        }

        return payment;
    }

    public bool Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<Payment> GetAll()
    {
        lock (_lock)
        {
            return _payments
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }
    }

    public Payment? GetById(Guid id)
    {
        lock (_lock)
        {
            return _payments.FirstOrDefault(p => p.Id == id);
        }
    }

    public bool Reject(Guid id)
    {
        lock (_lock)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);

            if (payment is null || payment.Status != PaymentStatus.PendingApproval)
            {
                return false;
            }

            payment.Status = PaymentStatus.Rejected;

            return true;
        }
    }
}
