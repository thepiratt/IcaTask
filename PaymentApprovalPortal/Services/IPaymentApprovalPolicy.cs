namespace PaymentApprovalPortal.Services;

public interface IPaymentApprovalPolicy
{
    bool RequiresApproval(decimal amount);
}
