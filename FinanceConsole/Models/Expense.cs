namespace FinanceConsole.Models
{
    /// <summary>INHERITANCE: Expense IS-A Transaction with a negative NetEffect.</summary>
    public class Expense : Transaction
    {
        public override string Kind => "EXPENSE";
        public override decimal NetEffect() => -Amount;
    }
}