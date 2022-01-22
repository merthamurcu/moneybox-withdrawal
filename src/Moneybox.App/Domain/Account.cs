using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal FundsLowLimit = 500m;
        public const decimal ApproachingPayInLimit = 500m;
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public Account(Guid id, User user, decimal balance, decimal withdrawn, decimal paidIn)
        {
            Id = id;
            User = user;
            Balance = balance;
            Withdrawn = withdrawn;
            PaidIn = paidIn;
        }

        public void Debit(INotificationService notificationService, decimal amount)
        {
            Balance -= amount;
            Withdrawn += amount;

            AvailableFundsCheck(notificationService, Balance, User.Email);
        }

        public void Credit(INotificationService notificationService, decimal amount)
        {
            Balance += amount;
            PaidIn += amount;

            PayInLimitCheck(notificationService, PaidIn, User.Email);
        }

        /// <summary>
        /// Checks PayIn limit to see if pay limit reached or approaching to the limit and notify them
        /// </summary>
        /// <param name="notificationService"></param>
        /// <param name="amount"></param>
        /// <param name="userEmail"></param>
        /// <exception cref="InvalidOperationException">Throws exception when account pay limit reached</exception>
        public static void PayInLimitCheck(INotificationService notificationService, decimal amount, string userEmail)
        {
            if (amount > PayInLimit)
                throw new InvalidOperationException("Account pay in limit reached");

            if (PayInLimit - amount < ApproachingPayInLimit)
                notificationService.NotifyApproachingPayInLimit(userEmail);
        }

        /// <summary>
        /// Checks funds to see if user has sufficient funds to make transfer or funds are low and notify them
        /// </summary>
        /// <param name="notificationService"></param>
        /// <param name="amount"></param>
        /// <param name="userEmail"></param>
        /// <exception cref="InvalidOperationException">Throws exception when account has insufficient funds to make transfer</exception>
        public static void AvailableFundsCheck(INotificationService notificationService, decimal amount, string userEmail)
        {
            if (amount < 0m)
                throw new InvalidOperationException("Insufficient funds to make transfer");

            if (amount < FundsLowLimit)
                notificationService.NotifyFundsLow(userEmail);
        }
    }
}
