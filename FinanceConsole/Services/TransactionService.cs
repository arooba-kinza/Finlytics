using System;
using System.Collections.Generic;
using FinanceConsole.Data;
using FinanceConsole.Exceptions;
using FinanceConsole.Interfaces;
using FinanceConsole.Models;

namespace FinanceConsole.Services
{
    /// <summary>
    /// Persists Income / Expense rows. Implements IRepository (ABSTRACTION).
    /// Uses Primary Constructor to leverage modern C# features.
    /// </summary>
    public class TransactionService(DatabaseContext db) : IRepository<Transaction>
    {
        private readonly DatabaseContext _db = db;

        public int Add(Transaction t)
        {
            if (t.Amount <= 0) throw new InvalidInputException("Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(t.Category)) throw new InvalidInputException("Category required.");

            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Transactions (AccountId, Kind, Category, Amount, Date, Note)
                                VALUES ($a, $k, $c, $amt, $d, $n);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$a", t.AccountId);
            cmd.Parameters.AddWithValue("$k", t.Kind);
            cmd.Parameters.AddWithValue("$c", t.Category);
            cmd.Parameters.AddWithValue("$amt", (double)t.Amount);
            cmd.Parameters.AddWithValue("$d", t.Date.ToString("o"));
            cmd.Parameters.AddWithValue("$n", t.Note ?? string.Empty);

            t.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return t.Id;
        }

        public IReadOnlyList<Transaction> GetAll(int accountId)
        {
            var list = new List<Transaction>();
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, AccountId, Kind, Category, Amount, Date, Note
                                FROM Transactions WHERE AccountId=$a ORDER BY Date DESC, Id DESC";
            cmd.Parameters.AddWithValue("$a", accountId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var kind = r.GetString(2);
                Transaction t = kind == "INCOME" ? new Income() : new Expense();
                t.Id = r.GetInt32(0);
                t.AccountId = r.GetInt32(1);
                t.Category = r.GetString(3);
                t.Amount = (decimal)r.GetDouble(4);
                t.Date = DateTime.Parse(r.GetString(5));
                t.Note = r.IsDBNull(6) ? string.Empty : r.GetString(6);
                list.Add(t);
            }
            return list;
        }

        public bool Delete(int id)
        {
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Transactions WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public void UpdateAmount(int id, decimal newAmount)
        {
            if (newAmount <= 0) throw new InvalidInputException("Amount must be > 0.");

            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Transactions SET Amount=$a WHERE Id=$id";
            cmd.Parameters.AddWithValue("$a", (double)newAmount);
            cmd.Parameters.AddWithValue("$id", id);

            if (cmd.ExecuteNonQuery() == 0) throw new NotFoundException($"Transaction {id} not found.");
        }
    }
}