using Microsoft.Extensions.Logging;
using PaymentApprovalPortal.Models;
using PaymentApprovalPortal.Repositories;

namespace PaymentApprovalPortal.Services;

public class PaymentService(IPaymentRepository repository, ILogger<PaymentService> logger, IConfiguration configuration, IAuditRepository auditService) : IPaymentService
{
    private readonly decimal approvalThreshold = configuration.GetValue<decimal>("PaymentSettings:ApprovalThreshold", 10_000m);

    public Payment Create(Payment payment)
    {
        if (string.IsNullOrWhiteSpace(payment.ReceiverName))
            throw new ArgumentException("Receiver name is required.", nameof(payment.ReceiverName));

        if (payment.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(payment.Amount));

        payment.MarkCreated();

        auditService.Log(payment.Id, "Payment created.");

        if (payment.Amount > approvalThreshold)
        {
            payment.MarkPendingApproval();

            auditService.Log(payment.Id, "Payment requires approval because amount exceeds threshold.");
        }
        else
        {
            payment.Execute();

            auditService.Log(payment.Id, "Payment executed immediately (at or below the approval threshold).");
        }

        repository.Add(payment);
        logger.LogInformation("Payment {PaymentId} created with status {Status}", payment.Id, payment.Status);

        return payment;
    }

    public bool Delete(Guid id)
    {
        logger.LogInformation("Delete requested for payment {PaymentId}", id);

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
        {
            logger.LogWarning("Approval failed for payment {PaymentId}: not found or not pending approval", id);
            return false;
        }    

        payment.Approve();
        auditService.Log(payment.Id, "Payment approved.");

        payment.Execute();
        auditService.Log(payment.Id, "Payment executed.");

        var updated = repository.Update(payment);
        if (updated)
        {
            logger.LogInformation("Payment {PaymentId} approved and executed", payment.Id);
        }

        return updated;
    }

    public bool Reject(Guid id)
    {

        var payment = repository.GetById(id);

        if (payment is null || payment.Status != PaymentStatus.PendingApproval)
        {
            logger.LogWarning("Rejection failed for payment {PaymentId}: not found or not pending approval",id);
            return false;
        }

        payment.Reject();
        auditService.Log(payment.Id, "Payment rejected.");

        var updated = repository.Update(payment);
        if (updated)
        {
            logger.LogInformation("Payment {PaymentId} rejected", payment.Id);
        }

        return updated;
    }
}
