using System;

namespace FinanceConsole.Models
{
    /// <summary>
    /// Represents a user account. Demonstrates ENCAPSULATION:
    /// all fields are private and exposed only via validated properties.
    /// </summary>
    public class Account
    {
        private int _id;
        private string _name = string.Empty;
        private decimal _monthlyIncome;
        private decimal _savingsRate; // 0..1, fraction of income user aims to save
        private DateTime _createdAt;

        public int Id
        {
            get => _id;
            set => _id = value; 
        }

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Name cannot be empty.")
                : value.Trim();
        }

        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set
            {
                if (value < 0) throw new ArgumentException("Monthly income cannot be negative.");
                _monthlyIncome = value;
            }
        }
         
        public decimal SavingsRate
        {
            get => _savingsRate;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Savings rate must be between 0 and 1.");
                _savingsRate = value;
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }
    }
}