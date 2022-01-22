using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moneybox.UnitTests.Domain;
using Moq;
using NUnit.Framework;

namespace Moneybox.UnitTests.Features
{
    public class TransferMoneyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Set_Correctly_And_Notifications_Called_As_Expected()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var accountRepositoryMock = new Mock<IAccountRepository>();
            var transferMoneyService = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            var fromAccount = AccountHelper.GetAccount();
            var toAccount = AccountHelper.GetAccount();

            accountRepositoryMock.Setup(x => x.GetAccountById(fromAccount.Id)).Returns(fromAccount);
            accountRepositoryMock.Setup(x => x.GetAccountById(toAccount.Id)).Returns(toAccount);

            // Act
            transferMoneyService.Execute(fromAccount.Id, toAccount.Id, 10m);

            // Assert
            Assert.AreEqual(90m, fromAccount.Balance);
            Assert.AreEqual(60m, fromAccount.Withdrawn);
            Assert.AreEqual(0m, fromAccount.PaidIn);

            Assert.AreEqual(110m, toAccount.Balance);
            Assert.AreEqual(50m, toAccount.Withdrawn);
            Assert.AreEqual(10m, toAccount.PaidIn);

            notificationServiceMock.Verify(m => m.NotifyFundsLow(fromAccount.User.Email), Times.Once);
            notificationServiceMock.Verify(m => m.NotifyApproachingPayInLimit(toAccount.User.Email), Times.Never);
            accountRepositoryMock.Verify(m => m.Update(fromAccount), Times.Once);
            accountRepositoryMock.Verify(m => m.Update(toAccount), Times.Once);
        }
    }
}
