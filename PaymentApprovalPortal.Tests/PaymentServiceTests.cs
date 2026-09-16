using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Globalization;
using FluentAssertions;
using PaymentApprovalPortal.Repositories;
using PaymentApprovalPortal.Services;
using PaymentApprovalPortal.Models;

namespace PaymentApprovalPortal.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepository;
    private readonly Mock<IAuditRepository> _auditRepository;
    private readonly Mock<ILogger<PaymentService>> _logger;
    private readonly Mock<IConfiguration> _configuration;

    public PaymentServiceTests()
    {
        _paymentRepository = new Mock<IPaymentRepository>();
        _auditRepository = new Mock<IAuditRepository>();
        _logger = new Mock<ILogger<PaymentService>>();
        _configuration = new Mock<IConfiguration>();
    }

    private PaymentService CreateService() => new PaymentService(
        _paymentRepository.Object,
        _logger.Object,
        _configuration.Object,
        _auditRepository.Object);

    private void SetupThreshold(decimal value)
    {
        var section = new Mock<IConfigurationSection>();
        section.Setup(s => s.Value).Returns(value.ToString(CultureInfo.InvariantCulture));
        _configuration.Setup(c => c.GetSection("PaymentSettings:ApprovalThreshold")).Returns(section.Object);
    }

    [Fact]
    public void Create_Executes_WhenAmountAtOrBelowThreshold()
    {
        SetupThreshold(10000m);

        var service = CreateService();

        var payment = new Payment { ReceiverName = "Harun", AccountNumber = "123", Amount = 10000m };

        var result = service.Create(payment);

        result.Status.Should().Be(PaymentStatus.Executed);
        _paymentRepository.Verify(r => r.Add(It.Is<Payment>(p => p.Id == payment.Id)), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("created"))), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("executed"))), Times.Once);
    }

    [Fact]
    public void Create_MarksPendingApproval_WhenAmountExceedsThreshold()
    {
        SetupThreshold(10000m);

        var service = CreateService();

        var payment = new Payment { ReceiverName = "Harun", AccountNumber = "321", Amount = 10001m };

        var result = service.Create(payment);

        result.Status.Should().Be(PaymentStatus.PendingApproval);
        _paymentRepository.Verify(r => r.Add(It.IsAny<Payment>()), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("requires approval"))), Times.Once);
    }

    [Fact]
    public void Create_InvalidReceiver_Throws()
    {
        SetupThreshold(10000m);

        var service = CreateService();

        var payment = new Payment { ReceiverName = "", AccountNumber = "000", Amount = 100m };

        Action act = () => service.Create(payment);

        act.Should().Throw<ArgumentException>().Where(e => e.ParamName == "ReceiverName");
        _paymentRepository.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public void Approve_Succeeds_WhenPending()
    {
        var payment = new Payment { ReceiverName = "Harun", AccountNumber = "555", Amount = 20000m };
        payment.MarkCreated();
        payment.MarkPendingApproval();

        _paymentRepository.Setup(r => r.GetById(payment.Id)).Returns(payment);
        _paymentRepository.Setup(r => r.Update(It.IsAny<Payment>())).Returns(true);
        SetupThreshold(10000m);

        var service = CreateService();

        var ok = service.Approve(payment.Id);

        ok.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Executed);
        _paymentRepository.Verify(r => r.Update(It.Is<Payment>(p => p.Id == payment.Id)), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("approved"))), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("executed"))), Times.Once);
    }

    [Fact]
    public void Approve_Fails_WhenNotPending()
    {
        var payment = new Payment { ReceiverName = "Harun", AccountNumber = "666", Amount = 100m };
        payment.MarkCreated();

        _paymentRepository.Setup(r => r.GetById(payment.Id)).Returns(payment);
        SetupThreshold(10000m);

        var service = CreateService();

        var ok = service.Approve(payment.Id);

        ok.Should().BeFalse();
        _paymentRepository.Verify(r => r.Update(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public void Reject_Succeeds_WhenPending()
    {
        var payment = new Payment { ReceiverName = "Harun", AccountNumber = "777", Amount = 20000m };
        payment.MarkCreated();
        payment.MarkPendingApproval();

        _paymentRepository.Setup(r => r.GetById(payment.Id)).Returns(payment);
        _paymentRepository.Setup(r => r.Update(It.IsAny<Payment>())).Returns(true);
        SetupThreshold(10000m);

        var service = CreateService();

        var ok = service.Reject(payment.Id);

        ok.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Rejected);
        _paymentRepository.Verify(r => r.Update(It.Is<Payment>(p => p.Id == payment.Id)), Times.Once);
        _auditRepository.Verify(a => a.Log(payment.Id, It.Is<string>(s => s.Contains("rejected"))), Times.Once);
    }

    [Fact]
    public void Delete_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        _paymentRepository.Setup(r => r.Delete(id)).Returns(true);

        SetupThreshold(10000m);
        var service = CreateService();

        var ok = service.Delete(id);

        ok.Should().BeTrue();
        _paymentRepository.Verify(r => r.Delete(id), Times.Once);
    }
}
