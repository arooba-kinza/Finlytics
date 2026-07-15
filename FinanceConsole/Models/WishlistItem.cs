using System;

namespace FinanceConsole.Models
{
    /// <summary>
    /// Something the user wants to buy (e.g. "Laptop").
    /// Used by the affordability planner.
    /// </summary>
    public class WishlistItem
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "Shopping";
        public decimal TargetPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}