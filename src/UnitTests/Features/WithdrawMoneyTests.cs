using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moneybox.UnitTests.Domain;
using Moq;
using NUnit.Framework;

namespace Moneybox.UnitTests.Features
{
    class WithdrawMoneyTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Should_Set_Correctly()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var accountRepositoryMock = new Mock<IAccountRepository>();
            var withdrawMoneyService = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            var fromAccount = AccountHelper.GetAccount();
            accountRepositoryMock.Setup(x => x.GetAccountById(fromAccount.Id)).Returns(fromAccount);

            // Act
            withdrawMoneyService.Execute(fromAccount.Id, 10m);

            // Assert
            Assert.AreEqual(90m, fromAccount.Balance);
            Assert.AreEqual(60m, fromAccount.Withdrawn);
            Assert.AreEqual(0m, fromAccount.PaidIn);

            notificationServiceMock.Verify(m => m.NotifyFundsLow(fromAccount.User.Email), Times.Once);
            accountRepositoryMock.Verify(m => m.Update(fromAccount), Times.Once);
        }
    }
}
