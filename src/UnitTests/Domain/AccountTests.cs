using Moneybox.App;
using Moneybox.App.Domain.Services;
using Moneybox.UnitTests.Domain;
using Moq;
using NUnit.Framework;
using System;

namespace Moneybox.UnitTests.Domains
{
    public class AccountTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Throw_Exception_When_Insufficient_Funds()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var account = AccountHelper.GetAccount();

            // Assert
            Assert.That(() => account.Debit(notificationServiceMock.Object, 150), Throws.Exception.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Insufficient funds to make transfer"));
        }

        [Test]
        public void Should_Throw_Exception_When_PayInLimit_Reached()
        {
            // Arrange
            var account = AccountHelper.GetAccount();
            var notificationServiceMock = new Mock<INotificationService>();

            // Assert
            Assert.That(() => account.Credit(notificationServiceMock.Object, Account.PayInLimit + 1), Throws.Exception.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Account pay in limit reached"));
        }

        [Test]
        public void Should_Debit_Correctly()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var account = AccountHelper.GetAccount();

            // Act
            account.Debit(notificationServiceMock.Object, 10m);

            // Assert
            Assert.AreEqual(90m, account.Balance);
            Assert.AreEqual(60m, account.Withdrawn); 
            notificationServiceMock.Verify(m => m.NotifyFundsLow(account.User.Email), Times.Once);
        }

    }
}