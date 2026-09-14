namespace PaymentApprovalPortal.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReceiverName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMessage { get; set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Draft;
    public DateTime CreatedDate { get; set; }
    public List<AuditEntry> AuditTrail { get; set; } = new();

    public void MarkCreated()
    {
        Status = PaymentStatus.Draft;
        AddAudit("Payment created.");
    }

    public void MarkPendingApproval()
    {
        Status = PaymentStatus.PendingApproval;
        AddAudit("Payment requires approval because amount exceeds threshold.");
    }

    public void Approve()
    {
        if (Status != PaymentStatus.PendingApproval)
            throw new InvalidOperationException("Only pending payments can be approved.");

        Status = PaymentStatus.Approved;
        AddAudit("Payment approved.");
    }

    public void Execute()
    {
        if (Status != PaymentStatus.Draft &&
            Status != PaymentStatus.Approved)
        {
            throw new InvalidOperationException("Payment cannot be executed from its current state.");
        }

        Status = PaymentStatus.Executed;
        AddAudit("Payment executed.");
    }

    public void Reject()
    {
        if (Status != PaymentStatus.PendingApproval)
            throw new InvalidOperationException("Only pending payments can be rejected.");

        Status = PaymentStatus.Rejected;
        AddAudit("Payment rejected.");
    }

    private void AddAudit(string description)
    {
        AuditTrail.Add(new AuditEntry(description));
    }
}
