using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces
{
    public interface IAdmin
    {
        public MsgDto ManageTax(TaxCalDto taxCalDto);
        public MsgDto ManageCalculations();
        public MsgDto ManagePayCode(PayCodeDto payCodeDto);
        public MsgDto AddSpecialRateEmp();
        public MsgDto AddSpecialTaxEmp();
    }
}
