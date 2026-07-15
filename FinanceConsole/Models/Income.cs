namespace FinanceConsole.Models
{
    /// <summary>INHERITANCE: Income IS-A Transaction with a positive NetEffect.</summary>
    public class Income : Transaction
    {
        public override string Kind => "INCOME";
        public override decimal NetEffect() => Amount;
    }
}