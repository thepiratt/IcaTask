using Microsoft.AspNetCore.Mvc;
using PaymentApprovalPortal.Models;
using PaymentApprovalPortal.Repositories;
using PaymentApprovalPortal.Services;

namespace PaymentApprovalPortal.Controllers
{
    public class PaymentsController(IPaymentService paymentService, IAuditRepository auditService) : Controller
    {
        public IActionResult Index()
        {
            var payments = paymentService.GetAll();

            var viewModels = payments
                .Select(payment => new PaymentDetailViewModel
                {
                    Payment = payment,
                    AuditEntries = auditService.GetByPaymentId(payment.Id)
                })
                .ToList();

            return View(viewModels);
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
                PaymentMessage = model.PaymentMessage
            });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(Guid id)
        {
            paymentService.Approve(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(Guid id)
        {
            paymentService.Reject(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            paymentService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
