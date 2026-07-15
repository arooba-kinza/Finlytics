using System;

namespace FinanceConsole.Exceptions
{
    /// <summary>
    /// Base exception type for all domain-level errors in the Finance app.
    /// Allows callers to catch domain errors separately from system errors.
    /// </summary>
    public class FinanceException : Exception
    {
        public FinanceException(string message) : base(message) { }
        public FinanceException(string message, Exception inner) : base(message, inner) { }
    }   

    /// <summary>Thrown when the user enters an invalid value (e.g. negative amount).</summary>
    public class InvalidInputException : FinanceException
    {
        public InvalidInputException(string message) : base(message) { }
    }

    /// <summary>Thrown when a requested record cannot be located in the database.</summary>
    public class NotFoundException : FinanceException
    {
        public NotFoundException(string message) : base(message) { }
    }
}