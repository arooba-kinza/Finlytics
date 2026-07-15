using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace FinanceConsole.Data
{
    /// <summary>
    /// Thin wrapper around the SQLite connection. Creates the DB file
    /// and schema on first run.
    /// </summary>
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string? dbPath = null)
        {
            dbPath ??= Path.Combine(AppContext.BaseDirectory, "finance.db");
            _connectionString = $"Data Source={dbPath}";
            Initialize();
        }

        public SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection(_connectionString);//created obj of built in sqlite class in pakage
            //feed it with your own credentials -coonectionstring
            conn.Open();
            return conn;
        }

        private void Initialize()
        {
            using var conn = OpenConnection(); //connects the databse with code
            using var cmd = conn.CreateCommand();// carry the blue print which creates a table//command reads the data 
            cmd.CommandText = @"    
                CREATE TABLE IF NOT EXISTS Accounts (
                    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name          TEXT    NOT NULL,
                    MonthlyIncome REAL    NOT NULL DEFAULT 0,
                    SavingsRate   REAL    NOT NULL DEFAULT 0.2,
                    CreatedAt     TEXT    NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Transactions (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountId INTEGER NOT NULL,
                    Kind      TEXT    NOT NULL,    -- 'INCOME' or 'EXPENSE'
                    Category  TEXT    NOT NULL,
                    Amount    REAL    NOT NULL,
                    Date      TEXT    NOT NULL,
                    Note      TEXT,
                    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS Wishlist (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountId   INTEGER NOT NULL,
                    Name        TEXT    NOT NULL,
                    Category    TEXT    NOT NULL,
                    TargetPrice REAL    NOT NULL,
                    CreatedAt   TEXT    NOT NULL,
                    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
                );
            ";
            cmd.ExecuteNonQuery();// actually physically build the table 
        }
    }
}