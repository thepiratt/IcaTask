using PaymentApprovalPortal.Models;
using System.Collections.Concurrent;

namespace PaymentApprovalPortal.Repositories;

public class InMemoryPaymentRepository(ILogger<InMemoryPaymentRepository> logger) : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> payments = new();

    public void Add(Payment payment)
    {
        payments[payment.Id] = payment;

        logger.LogDebug("Payment {PaymentId} added to repository", payment.Id);
    }

    public bool Delete(Guid id)
    {
        var removed = payments.TryRemove(id, out _);

        if (removed)
            logger.LogDebug("Payment {PaymentId} removed from repository", id);

        return removed;
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
        if (!payments.TryGetValue(payment.Id, out var existing))
            return false;

        var updated = payments.TryUpdate(payment.Id, payment, existing);
        if (updated)
            logger.LogDebug("Payment {PaymentId} updated in repository", payment.Id);

        return updated;
    }
}
