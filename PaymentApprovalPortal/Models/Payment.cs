namespace PaymentApprovalPortal.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReceiverName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMessage { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Draft;
    public DateTime CreatedDate { get; set; }
}
