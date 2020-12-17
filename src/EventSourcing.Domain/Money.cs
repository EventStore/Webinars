using System.Linq;
using EventSourcing.Lib;

namespace EventSourcing.Domain {
    public record Money {
        public double Amount   { get; internal init; }
        public string Currency { get; internal init; }

        static readonly string[] SupportedCurrencies = {"USD", "EUR", "GPB"};

        internal Money() { }

        public Money(double amount, string currency) {
            if (!SupportedCurrencies.Contains(currency)) throw new DomainException($"Unsupported currency: {currency}");

            Amount   = amount;
            Currency = currency;
        }

        public bool IsSameCurrency(Money another) => Currency == another.Currency;

        public static Money operator -(Money one, Money another) {
            if (!one.IsSameCurrency(another)) throw new DomainException("Cannot operate on different currencies");

            return new Money(one.Amount - another.Amount, one.Currency);
        }

        public static implicit operator double(Money money) => money.Amount;
    }
}
