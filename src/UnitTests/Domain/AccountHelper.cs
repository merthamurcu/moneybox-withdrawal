using Moneybox.App;
using System;

namespace Moneybox.UnitTests.Domain
{
    public static class AccountHelper
    {
        public static Account GetAccount()
        {
            return new Account(
               id: Guid.NewGuid(),
               user: new User(Guid.NewGuid(), "mert", "mert@moneybox.com"),
               balance: 100,
               withdrawn: 50,
               paidIn: 0
           );
        }
    }
}
