
using System.Text.Json.Serialization;

namespace ELOR.Razzle.Data.Enums
{
    public enum CashFlowType : byte
    {
        [JsonStringEnumMemberName("spent")]        Spent = 0,
        [JsonStringEnumMemberName("earned")]       Earned = 1,
        [JsonStringEnumMemberName("debtRepaid")]   DebtRepaid = 2,   // Current user paid off money I owed
        [JsonStringEnumMemberName("loanTaken")]    LoanTaken = 3,    // ...borrowed money
        [JsonStringEnumMemberName("loanGiven")]    LoanGiven = 4,    // ...lent money
        [JsonStringEnumMemberName("debtReturned")] DebtReturned = 5  // Someone returned money they owed from current user
    }

    [Flags]
    public enum NoteFlags : byte
    {
        None = 0,
    }

    public enum TagType : byte
    {
        [JsonStringEnumMemberName("default")] Default = 0,
        [JsonStringEnumMemberName("place")] Place = 1,
        [JsonStringEnumMemberName("person")] Person = 2,
        [JsonStringEnumMemberName("work")] Work = 3
    }

    [Flags]
    public enum TaskFlags : byte
    {
        None = 0,
        IsCompleted = 1 << 0,
    }
}