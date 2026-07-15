using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinanceConsole.Models;

namespace FinanceConsole.Services
{
    /// <summary>
    /// Aggregates transactions into reports, overspending alerts,
    /// and affordability forecasts for wishlist items.
    /// Uses Primary Constructor to satisfy modern static analysis rules.
    /// </summary>
    public class ReportService(TransactionService tx)
    {
        private readonly TransactionService _tx = tx;

        public decimal CurrentSavings(int accountId)
        {
            return _tx.GetAll(accountId).Sum(t => t.NetEffect());
        }

        public string MonthlyReport(Account acc, int year, int month)
        {
            var all = _tx.GetAll(acc.Id)
                         .Where(t => t.Date.Year == year && t.Date.Month == month)
                         .ToList();

            decimal income = all.OfType<Income>().Sum(t => t.Amount);
            decimal expense = all.OfType<Expense>().Sum(t => t.Amount);
            decimal net = income - expense;

            var sb = new StringBuilder();
            sb.AppendLine($"========== MONTHLY REPORT — {year}-{month:00} ==========");
            sb.AppendLine($"Account: {acc.Name} (#{acc.Id})");
            sb.AppendLine($"Income : {income,12:0.00}");
            sb.AppendLine($"Expense: {expense,12:0.00}");
            sb.AppendLine($"Net    : {net,12:0.00}");
            sb.AppendLine();
            sb.AppendLine("-- By Category --");
            foreach (var g in all.GroupBy(t => t.Category).OrderBy(g => g.Key))
            {
                decimal inc = g.OfType<Income>().Sum(x => x.Amount);
                decimal exp = g.OfType<Expense>().Sum(x => x.Amount);
                sb.AppendLine($"  {g.Key,-15}  income {inc,10:0.00}   expense {exp,10:0.00}");
            }
            sb.AppendLine();
            sb.AppendLine("-- Transactions --");
            if (all.Count == 0) sb.AppendLine("  (none)");
            foreach (var t in all.OrderBy(t => t.Date))
                sb.AppendLine("  " + t.Describe(true));

            if (expense > income && income > 0)
                sb.AppendLine($"\n  ALERT: You overspent by {expense - income:0.00} this month.");

            if (acc.MonthlyIncome > 0 && expense > acc.MonthlyIncome * 0.9m)
                sb.AppendLine($"  ALERT: Expenses exceeded 90% of baseline monthly income ({acc.MonthlyIncome:0.00}).");

            return sb.ToString();
        }

        public string AffordabilityPlan(Account acc, WishlistItem item)
        {
            decimal currentSavings = CurrentSavings(acc.Id);
            var sb = new StringBuilder();
            sb.AppendLine($"=== Affordability check: {item.Name} ({item.Category}) ===");
            sb.AppendLine($"Target price   : {item.TargetPrice,12:0.00}");
            sb.AppendLine($"Current savings: {currentSavings,12:0.00}");

            if (currentSavings >= item.TargetPrice)
            {
                sb.AppendLine("VERDICT: You can buy it NOW. (Realistic)");
                return sb.ToString();
            }

            decimal shortfall = item.TargetPrice - currentSavings;
            decimal monthlySaving = acc.MonthlyIncome * acc.SavingsRate;
            sb.AppendLine($"Shortfall      : {shortfall,12:0.00}");
            sb.AppendLine($"Plan: save {acc.SavingsRate * 100:0}% of {acc.MonthlyIncome:0.00} = {monthlySaving:0.00} per month.");

            if (monthlySaving <= 0)
            {
                sb.AppendLine("VERDICT: Not realistic — set a positive income and savings rate first.");
                return sb.ToString();
            }

            int roundedMonths = (int)Math.Ceiling((double)(shortfall / monthlySaving));
            sb.AppendLine($"VERDICT: You can afford it in approximately {roundedMonths} month(s).");

            if (roundedMonths > 12)
            {
                decimal neededRate = (shortfall / 12m) / acc.MonthlyIncome;
                if (neededRate <= 1m)
                    sb.AppendLine($"Tip: raise savings rate to {neededRate * 100:0}% to buy within 12 months.");
                else
                    sb.AppendLine("Tip: this item is far beyond your monthly income; consider a cheaper alternative.");
            }

            return sb.ToString();
        }
    }
}