using System.ComponentModel.DataAnnotations;

namespace PaymentApprovalPortal.Models;

public class CreatePaymentViewModel
{
    [Required(ErrorMessage = "Receiver name is required.")]
    [StringLength(50, ErrorMessage = "Receiver name may not exceed 50 characters.")]
    [Display(Name = "Receiver name")]
    public string ReceiverName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Account number is required.")]
    [RegularExpression(@"^\d{6,}$", ErrorMessage = "Account number must contain only digits and be at least 6 digits long.")]
    [Display(Name = "Account number")]
    public string AccountNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Amount is required.")]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than zero.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Amount can have at most two decimal places.")]
    [Display(Name = "Amount (SEK)")]
    public decimal Amount { get; set; }
    [StringLength(140, ErrorMessage = "Payment message may not exceed 140 characters.")]
    [Display(Name = "Payment message")]
    public string? PaymentMessage { get; set; }
}
