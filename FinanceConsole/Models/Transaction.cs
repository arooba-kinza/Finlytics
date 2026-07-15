using System;

namespace FinanceConsole.Models
{
    /// <summary>
    /// Abstract base class for all monetary transactions.
    /// Demonstrates ABSTRACTION (cannot be instantiated) and
    /// is the parent for INHERITANCE by Income / Expense.
    /// </summary>
    public abstract class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Category { get; set; } = "General";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Discriminator persisted in SQLite so we can rehydrate the right subclass.
        /// </summary>
        public abstract string Kind { get; }

        /// <summary>
        /// POLYMORPHISM: each subclass returns its signed effect on net cash flow.
        /// </summary>
        public abstract decimal NetEffect();//WHY does transaction affect user net balance..

        /// <summary>Method OVERLOADING (polymorphism): two Describe signatures.</summary>
        public virtual string Describe() =>
            $"[{Kind,-7}] {Date:yyyy-MM-dd}  {Category,-12}  {Amount,10:0.00}  {Note}";

        public string Describe(bool withId) =>
            withId ? $"#{Id,-4} {Describe()}" : Describe();
    }
}