namespace PaymentApprovalPortal.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReceiverName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMessage { get; set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public void MarkCreated()
    {
        Status = PaymentStatus.Draft;
    }

    public void MarkPendingApproval()
    {
        Status = PaymentStatus.PendingApproval;
    }

    public void Approve()
    {
        if (Status != PaymentStatus.PendingApproval)
            throw new InvalidOperationException("Only pending payments can be approved.");

        Status = PaymentStatus.Approved;
    }

    public void Execute()
    {
        if (Status != PaymentStatus.Draft && Status != PaymentStatus.Approved)
            throw new InvalidOperationException("Payment cannot be executed from its current state.");
        
        Status = PaymentStatus.Executed;
    }

    public void Reject()
    {
        if (Status != PaymentStatus.PendingApproval)
            throw new InvalidOperationException("Only pending payments can be rejected.");

        Status = PaymentStatus.Rejected;
    }
}
