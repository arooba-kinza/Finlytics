using System.Collections.Generic;

namespace FinanceConsole.Interfaces
{
    /// <summary>
    /// Generic repository contract (Abstraction pillar).
    /// Concrete services implement this for each entity type.
    /// </summary>
    public interface IRepository<T>
    {
        int Add(T item);
        IReadOnlyList<T> GetAll(int accountId); //geetall will give all transactions with that id
        bool Delete(int id);
    }
}