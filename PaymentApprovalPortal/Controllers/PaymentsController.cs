using Microsoft.AspNetCore.Mvc;
using PaymentApprovalPortal.Models;
using PaymentApprovalPortal.Services;

namespace PaymentApprovalPortal.Controllers
{
    public class PaymentsController(IPaymentService paymentService) : Controller
    {
        public IActionResult Index()
        {
            return View(paymentService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePaymentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreatePaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var payment = paymentService.Create(new Payment
            {
                ReceiverName = model.ReceiverName.Trim(),
                AccountNumber = model.AccountNumber.Trim(),
                Amount = model.Amount,
                PaymentMessage = string.IsNullOrWhiteSpace(model.PaymentMessage)
                    ? null
                    : model.PaymentMessage.Trim()
            });

            TempData["Success"] = payment.Status == PaymentStatus.Executed
                ? $"Payment of {payment.Amount:N2} SEK was executed immediately (at or below the 10 000 SEK threshold)."
                : $"Payment of {payment.Amount:N2} SEK was created and is pending approval.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(Guid id)
        {
            if (paymentService.Approve(id))
            {
                TempData["Success"] = "Payment approved and executed.";
            }
            else
            {
                TempData["Error"] = "Payment could not be approved. Only payments pending approval can be approved.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(Guid id)
        {
            if (paymentService.Reject(id))
            {
                TempData["Success"] = "Payment rejected.";
            }
            else
            {
                TempData["Error"] = "Payment could not be rejected. Only payments pending approval can be rejected.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            if (paymentService.Delete(id))
            {
                TempData["Success"] = "Payment deleted.";
            }
            else
            {
                TempData["Error"] = "Payment not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
