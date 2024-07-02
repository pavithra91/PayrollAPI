using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces
{
    public interface IAdmin
    {
        public Task<MsgDto> GetTaxDetails();
        public Task<MsgDto> GetTaxDetailsById(int id);
        public Task<MsgDto> CreateTaxCalculation(TaxCalDto taxCalDto);
        public Task<MsgDto> UpdateTax(TaxCalDto taxCalDto);

        public Task<MsgDto> GetPayCodes();
        public Task<MsgDto> GetPayCodesById(int id);
        public Task<MsgDto> CreatePayCode(PayCodeDto payCodeDto);
        public Task<MsgDto> UpdatePayCode(PayCodeDto payCodeDto);

        public Task<MsgDto> GetCalculations();
        public Task<MsgDto> GetCalculationsById(int id);
        public Task<MsgDto> CreateCalculation(CalDto calDto);
        public Task<MsgDto> UpdateCalculation(CalDto calDto);
        public Task<MsgDto> DeleteCalculation(CalDto calDto);

        public Task<MsgDto> GetSplRateEmp();
        public Task<MsgDto> GetSplRateEmpById(int id);
        public Task<MsgDto> CreateSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto);
        public Task<MsgDto> UpdateSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto);
        public Task<MsgDto> DeleteSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto);

        public Task<MsgDto> GetSplTaxEmp();
        public Task<MsgDto> GetSplTaxEmpById(int id);
        public Task<MsgDto> CreateSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto);
        public Task<MsgDto> UpdateSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto);
        public Task<MsgDto> DeleteSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto);

        
        public Task<MsgDto> GetOTDetails(int period, int companyCode);
        public Task<MsgDto> GetUnrecoveredDetails(int period, int companyCode);
        public Task<MsgDto> GetLumpSumTaxDetails(int period, int companyCode);
        public Task<MsgDto> ResetData(ResetDto resetDto);

        public Task<MsgDto> GetSystemVariables();
        public Task<MsgDto> CreateSystemVariable(SysVariableDto sysVariableDto);
        public Task<MsgDto> UpdateSystemVariable(SysVariableDto sysVariableDto);
    }
}
