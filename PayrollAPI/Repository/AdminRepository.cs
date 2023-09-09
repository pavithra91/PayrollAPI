using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;

namespace PayrollAPI.Repository
{
    public class AdminRepository : IAdmin
    {
        private readonly DBConnect _context;
        public AdminRepository(DBConnect context)
        {
            _context = context;
        }
        public MsgDto ManageTax(TaxCalDto taxCalDto)
        {
            MsgDto _msg = new MsgDto();

            if (taxCalDto.flag == 'N')
            {
                var _tax = new Tax_Calculation
                {
                    calFormula = taxCalDto.calFormula,
                    description = taxCalDto.description,
                    range = taxCalDto.range,
                    status = true,
                    createdBy = taxCalDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_tax);
                _context.SaveChanges();

                return _msg;
            }
            else if (taxCalDto.flag == 'U')
            {
                var _tax = _context.Tax_Calculation.FirstOrDefault(o => o.id == taxCalDto.id);
                if (_tax != null)
                {
                    _tax.calFormula = taxCalDto.calFormula;
                    _tax.description = taxCalDto.description;
                    _tax.status = taxCalDto.status;
                    _tax.range = taxCalDto.range;
                    _tax.lastUpdateBy = taxCalDto.lastUpdateBy;
                    _tax.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Tax updated Successfully";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Tax Code Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else if (taxCalDto.flag == 'D')
            {
                // Not Implemented
                return _msg;
            }
            else
            {
                return _msg;
            }
        }
        public MsgDto ManagePayCode(PayCodeDto payCodeDto)
        {
            MsgDto _msg = new MsgDto();

            if (payCodeDto.flag == 'N')
            {
                var _payCode = new PayCode
                {
                    calCode = payCodeDto.calCode,
                    companyCode = payCodeDto.companyCode,
                    description = payCodeDto.description,
                    payCode = payCodeDto.payCode,
                    payCategory = payCodeDto.payCategory,
                    rate = payCodeDto.rate,
                    isTaxableGross = true,
                    createdBy = payCodeDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_payCode);
                _context.SaveChanges();

                return _msg;
            }
            else if(payCodeDto.flag == 'U')
            {
                var _payCode = _context.PayCode.FirstOrDefault(o => o.id == payCodeDto.id);
                if (_payCode != null)
                {
                    _payCode.calCode = payCodeDto.calCode;
                    _payCode.description = payCodeDto.description;
                    _payCode.payCategory = payCodeDto.payCategory;
                    _payCode.rate = payCodeDto.rate;
                    _payCode.isTaxableGross = payCodeDto.isTaxableGross;
                    _payCode.payCode = payCodeDto.payCode;
                    _payCode.lastUpdateBy = payCodeDto.lastUpdateBy;
                    _payCode.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Pay code updated Successfully";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Paycode Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else if (payCodeDto.flag == 'D')
            {
                // Not Implemented
                return _msg;
            }
            else
            {
                return _msg;
            }
        }
        public MsgDto ManageCalculations()
        {
            throw new NotImplementedException();
        }
        public MsgDto AddSpecialRateEmp()
        {
            throw new NotImplementedException();
        }
        public MsgDto AddSpecialTaxEmp()
        {
            throw new NotImplementedException();
        }
    }
}
