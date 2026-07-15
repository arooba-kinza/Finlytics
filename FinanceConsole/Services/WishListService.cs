using System;
using System.Collections.Generic;
using FinanceConsole.Data;
using FinanceConsole.Exceptions;
using FinanceConsole.Interfaces;
using FinanceConsole.Models;

namespace FinanceConsole.Services
{
    /// <summary>
    /// Manages wishlist item persistence. Uses Primary Constructor modern C# syntax.
    /// </summary>
    public class WishlistService(DatabaseContext db) : IRepository<WishlistItem>
    {
        private readonly DatabaseContext _db = db;

        public int Add(WishlistItem w)
        {
            if (w.TargetPrice <= 0) throw new InvalidInputException("Target price must be > 0.");
            if (string.IsNullOrWhiteSpace(w.Name)) throw new InvalidInputException("Name required.");

            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Wishlist (AccountId, Name, Category, TargetPrice, CreatedAt)
                                VALUES ($a, $n, $c, $p, $d);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$a", w.AccountId);
            cmd.Parameters.AddWithValue("$n", w.Name);
            cmd.Parameters.AddWithValue("$c", w.Category);
            cmd.Parameters.AddWithValue("$p", (double)w.TargetPrice);
            cmd.Parameters.AddWithValue("$d", w.CreatedAt.ToString("o"));

            w.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return w.Id;
        }

        public IReadOnlyList<WishlistItem> GetAll(int accountId)
        {
            var list = new List<WishlistItem>();
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, AccountId, Name, Category, TargetPrice, CreatedAt
                                FROM Wishlist WHERE AccountId=$a ORDER BY Id";
            cmd.Parameters.AddWithValue("$a", accountId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new WishlistItem
                {
                    Id = r.GetInt32(0),
                    AccountId = r.GetInt32(1),
                    Name = r.GetString(2),
                    Category = r.GetString(3),
                    TargetPrice = (decimal)r.GetDouble(4),
                    CreatedAt = DateTime.Parse(r.GetString(5))
                });
            }
            return list;
        }

        public bool Delete(int id)
        {
            using var conn = _db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Wishlist WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}