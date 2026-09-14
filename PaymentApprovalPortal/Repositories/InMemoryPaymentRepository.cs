using PaymentApprovalPortal.Models;
using System.Collections.Concurrent;

namespace PaymentApprovalPortal.Repositories;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> payments = new();

    public void Add(Payment payment)
    {
        payments[payment.Id] = payment;
    }

    public bool Delete(Guid id)
    {
        return payments.TryRemove(id, out _);
    }

    public IReadOnlyList<Payment> GetAll()
    {
        return payments.Values.OrderByDescending(p => p.CreatedDate).ToList();
    }

    public Payment? GetById(Guid id)
    {
        payments.TryGetValue(id, out var payment);
        return payment;
    }

    public bool Update(Payment payment)
    {
        if (!payments.ContainsKey(payment.Id))
            return false;

        payments[payment.Id] = payment;
        return true;
    }
}
