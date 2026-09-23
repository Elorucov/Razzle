
namespace ELOR.Razzle.Data.Enums
{
    public enum CashFlowType : byte
    {
        Spent = 0,
        Earned = 1,
        DebtRepaid = 2,   // Current user paid off money I owed
        LoanTaken = 3,    // ...borrowed money
        LoanGiven = 4,    // ...lent money
        DebtReturned = 5  // Someone returned money they owed from current user
    }

    [Flags]
    public enum NoteFlags : byte
    {
        None = 0,
    }

    public enum TagType : byte
    {
        Default = 0,
        Location = 1,
        Person = 2,
        Work = 3
    }

    [Flags]
    public enum TaskFlags : byte
    {
        None = 0,
        IsCompleted = 1 << 0,
    }
}