using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.DTO
{
    public sealed class CashFlowDTO
    {
        public CashFlowType FlowType { get; set; }
        public uint Amount { get; set; }
    }
}
