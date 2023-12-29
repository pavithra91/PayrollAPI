using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces
{
    public interface IAdmin
    {
        public MsgDto ManageTax(TaxCalDto taxCalDto);
        public Task<MsgDto> UpdateTax(TaxCalDto taxCalDto);
        public MsgDto ManageCalculations(CalDto calDto);
        public MsgDto ManagePayCode(PayCodeDto payCodeDto);
        public MsgDto ManageSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto);
        public MsgDto ManageSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto);
    }
}
