using Microsoft.Extensions.Configuration;

namespace PaymentApprovalPortal.Services;

public class PaymentApprovalPolicy(IConfiguration configuration) : IPaymentApprovalPolicy
{
    private readonly decimal approvalThreshold = configuration.GetValue<decimal>("PaymentSettings:ApprovalThreshold", 10_000m);

    public bool RequiresApproval(decimal amount)
        => amount > approvalThreshold;
}
