using System;
using System.Linq;
using FinanceConsole.Data;
using FinanceConsole.Models;
using FinanceConsole.Services;
using FinanceConsole.UI;

namespace FinanceConsole
{
    /// <summary>
    /// Finance Analytics Console — entry point.
    /// Demonstrates the four pillars of OOP:
    ///   Encapsulation (Account, Transaction)
    ///   Inheritance   (Income, Expense : Transaction)
    ///   Polymorphism  (NetEffect override; Describe overload)
    ///   Abstraction   (abstract Transaction, IRepository<T>)
    /// Plus structured exception handling throughout the UI layer.
    /// </summary>
    public static class Program
    {
        private static DatabaseContext _db = null!;
        private static AccountService _accounts = null!;
        private static TransactionService _tx = null!;
        private static WishlistService _wish = null!;
        private static ReportService _reports = null!;
        private static Account? _current;

        public static void Main()
        {
            Console.Title = "Finance Analytics Console";
            _db = new DatabaseContext();
            _accounts = new AccountService(_db);
            _tx = new TransactionService(_db);
            _wish = new WishlistService(_db);
            _reports = new ReportService(_tx);

            Console.WriteLine("=============================================");
            Console.WriteLine("   FINANCE ANALYTICS CONSOLE  (.NET + SQLite)");
            Console.WriteLine("=============================================\n");

            SelectOrCreateAccount();

            while (true)
            {
                PrintMenu();
                var choice = ConsoleHelpers.ReadInt("Choose: ", 0, 10);
                Console.WriteLine();

                switch (choice)
                {
                    case 1: ConsoleHelpers.Safe(AddIncome); break;
                    case 2: ConsoleHelpers.Safe(AddExpense); break;
                    case 3: ConsoleHelpers.Safe(ListTransactions); break;
                    case 4: ConsoleHelpers.Safe(UpdateTransaction); break;
                    case 5: ConsoleHelpers.Safe(DeleteTransaction); break;
                    case 6: ConsoleHelpers.Safe(AddWishlistItem); break;
                    case 7: ConsoleHelpers.Safe(ShowAffordability); break;
                    case 8: ConsoleHelpers.Safe(ShowMonthlyReport); break;
                    case 9: ConsoleHelpers.Safe(UpdateIncomeRate); break;
                    case 10: ConsoleHelpers.Safe(SwitchAccount); break;
                    case 0: Console.WriteLine("Goodbye!"); return;
                }
                ConsoleHelpers.Pause();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine($"\n--- Account: {_current!.Name} (#{_current.Id})  |  Savings so far: {_reports.CurrentSavings(_current.Id):0.00} ---");
            Console.WriteLine(" 1. Add income");
            Console.WriteLine(" 2. Add expense");
            Console.WriteLine(" 3. List transactions");
            Console.WriteLine(" 4. Update transaction amount");
            Console.WriteLine(" 5. Delete transaction");
            Console.WriteLine(" 6. Add wishlist item (something I want to buy)");
            Console.WriteLine(" 7. Check affordability of a wishlist item");
            Console.WriteLine(" 8. Monthly report");
            Console.WriteLine(" 9. Update monthly income / savings rate");
            Console.WriteLine("10. Switch account");
            Console.WriteLine(" 0. Exit");
        }

        // ---------- Account flow ----------
        private static void SelectOrCreateAccount()
        {
            var existing = _accounts.GetAll();
            if (existing.Count == 0)
            {
                Console.WriteLine("No accounts found. Let's create one.\n");
                CreateAccount();
                return;
            }

            Console.WriteLine("Existing accounts:");
            foreach (var a in existing)
                Console.WriteLine($"  #{a.Id}  {a.Name}  income={a.MonthlyIncome:0.00}  rate={a.SavingsRate * 100:0}%");

            Console.WriteLine("  0   <Create new account>");
            var id = ConsoleHelpers.ReadInt("Select account id: ", 0);
            if (id == 0) CreateAccount();
            else _current = _accounts.GetById(id);
        }

        private static void CreateAccount()
        {
            var name = ConsoleHelpers.ReadString("Your name: ");
            var inc = ConsoleHelpers.ReadDecimal("Monthly income: ", 0);
            var rate = ConsoleHelpers.ReadDecimal("Target savings rate (0..1, e.g. 0.2): ", 0, 1);
            _current = _accounts.Create(name, inc, rate);
            Console.WriteLine($"Created account #{_current.Id}.");
        }

        private static void SwitchAccount() => SelectOrCreateAccount();

        // ---------- Transactions ----------
        private static void AddIncome()
        {
            var cat = ConsoleHelpers.ReadString("Category (e.g. Salary, Freelance): ");
            var amt = ConsoleHelpers.ReadDecimal("Amount: ", 0.01m);
            var note = ConsoleHelpers.ReadString("Note (optional): ", allowEmpty: true);
            var id = _tx.Add(new Income { AccountId = _current!.Id, Category = cat, Amount = amt, Note = note });
            Console.WriteLine($"Saved income #{id}.");
        }

        private static void AddExpense()
        {
            var cat = ConsoleHelpers.ReadString("Category (Food, Rent, Shopping, ...): ");
            var amt = ConsoleHelpers.ReadDecimal("Amount: ", 0.01m);
            var note = ConsoleHelpers.ReadString("Note (optional): ", allowEmpty: true);
            var id = _tx.Add(new Expense { AccountId = _current!.Id, Category = cat, Amount = amt, Note = note });
            Console.WriteLine($"Saved expense #{id}.");

            // Live overspending alert vs savings
            var savings = _reports.CurrentSavings(_current.Id);
            if (savings < 0)
                Console.WriteLine($"  ALERT: Your overall balance is negative ({savings:0.00}).");
        }

        private static void ListTransactions()
        {
            var list = _tx.GetAll(_current!.Id);
            if (list.Count == 0) { Console.WriteLine("(no transactions yet)"); return; }

            // Polymorphism: each subclass formats itself.
            foreach (var t in list) Console.WriteLine(t.Describe(true));
        }

        private static void UpdateTransaction()
        {
            ListTransactions();
            var id = ConsoleHelpers.ReadInt("Transaction id to update: ", 1);
            var amt = ConsoleHelpers.ReadDecimal("New amount: ", 0.01m);
            _tx.UpdateAmount(id, amt);
            Console.WriteLine("Updated.");
        }

        private static void DeleteTransaction()
        {
            var id = ConsoleHelpers.ReadInt("Transaction id to delete: ", 1);
            Console.WriteLine(_tx.Delete(id) ? "Deleted." : "Not found.");
        }

        // ---------- Wishlist / affordability ----------
        private static void AddWishlistItem()
        {
            var name = ConsoleHelpers.ReadString("Item name (e.g. Laptop): ");
            var cat = ConsoleHelpers.ReadString("Category (Shopping, Electronics, ...): ");
            var price = ConsoleHelpers.ReadDecimal("Target price: ", 0.01m);
            var id = _wish.Add(new WishlistItem
            {
                AccountId = _current!.Id,
                Name = name,
                Category = cat,
                TargetPrice = price
            });
            Console.WriteLine($"Added wishlist item #{id}.");
        }

        private static void ShowAffordability()
        {
            var items = _wish.GetAll(_current!.Id);
            if (items.Count == 0) { Console.WriteLine("Wishlist is empty. Add an item first (option 6)."); return; }

            foreach (var w in items)
                Console.WriteLine($"  #{w.Id}  {w.Name,-20}  {w.Category,-12}  {w.TargetPrice:0.00}");

            var id = ConsoleHelpers.ReadInt("Pick wishlist id: ", 1);
            var item = items.FirstOrDefault(x => x.Id == id)
                       ?? throw new Exceptions.NotFoundException("Wishlist item not found.");

            Console.WriteLine();
            Console.WriteLine(_reports.AffordabilityPlan(_current, item));
        }

        // ---------- Reports ----------
        private static void ShowMonthlyReport()
        {
            var year = ConsoleHelpers.ReadInt("Year (e.g. 2026): ", 2000, 2100);
            var month = ConsoleHelpers.ReadInt("Month (1-12): ", 1, 12);
            Console.WriteLine();
            Console.WriteLine(_reports.MonthlyReport(_current!, year, month));
        }

        private static void UpdateIncomeRate()
        {
            var inc = ConsoleHelpers.ReadDecimal("New monthly income: ", 0);
            var rate = ConsoleHelpers.ReadDecimal("New savings rate (0..1): ", 0, 1);
            _accounts.UpdateIncomeAndRate(_current!.Id, inc, rate);
            _current = _accounts.GetById(_current.Id);
            Console.WriteLine("Updated.");
        }
    }
}