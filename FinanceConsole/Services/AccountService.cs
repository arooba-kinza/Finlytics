using System;
using System.Collections.Generic;
using FinanceConsole.Data;
using FinanceConsole.Exceptions;
using FinanceConsole.Models;

namespace FinanceConsole.Services
{
    /// <summary>
    /// Repository class using Primary Constructor to comply with modern C# standards.
    /// </summary>
    public class AccountService(DatabaseContext db)
    {
        private readonly DatabaseContext _db = db;

        public Account Create(string name, decimal monthlyIncome, decimal savingsRate)
        {
            var acc = new Account
            {
                Name = name,
                MonthlyIncome = monthlyIncome,
                SavingsRate = savingsRate,
                CreatedAt = DateTime.Now
            };

            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Accounts (Name, MonthlyIncome, SavingsRate, CreatedAt)
                                VALUES ($n, $i, $r, $c);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$n", acc.Name);
            cmd.Parameters.AddWithValue("$i", (double)acc.MonthlyIncome);
            cmd.Parameters.AddWithValue("$r", (double)acc.SavingsRate);
            cmd.Parameters.AddWithValue("$c", acc.CreatedAt.ToString("o"));

            acc.Id = Convert.ToInt32(cmd.ExecuteScalar());   //read move to next phase
            return acc;
        }

        public List<Account> GetAll()
        {
            var list = new List<Account>();
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, MonthlyIncome, SavingsRate, CreatedAt FROM Accounts ORDER BY Id";

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Account
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    MonthlyIncome = (decimal)r.GetDouble(2),
                    SavingsRate = (decimal)r.GetDouble(3),
                    CreatedAt = DateTime.Parse(r.GetString(4))
                });
            }
            return list;
        }

        public Account GetById(int id)
        {
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, MonthlyIncome, SavingsRate, CreatedAt FROM Accounts WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) throw new NotFoundException($"Account {id} not found.");

            return new Account
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                MonthlyIncome = (decimal)r.GetDouble(2),
                SavingsRate = (decimal)r.GetDouble(3),
                CreatedAt = DateTime.Parse(r.GetString(4))
            };
        }

        public void UpdateIncomeAndRate(int id, decimal newIncome, decimal newRate)
        {
            if (newIncome < 0) throw new InvalidInputException("Income must be >= 0.");
            if (newRate < 0 || newRate > 1) throw new InvalidInputException("Rate must be 0..1.");

            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET MonthlyIncome=$i, SavingsRate=$r WHERE Id=$id";
            cmd.Parameters.AddWithValue("$i", (double)newIncome);
            cmd.Parameters.AddWithValue("$r", (double)newRate);
            cmd.Parameters.AddWithValue("$id", id);

            if (cmd.ExecuteNonQuery() == 0) throw new NotFoundException($"Account {id} not found.");
        }
    }
}