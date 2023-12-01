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
        public MsgDto ManageCalculations(CalDto calDto)
        {
            MsgDto _msg = new MsgDto();

            if (calDto.flag == 'N')
            {
                var _cal = new Calculation
                {
                    calCode = calDto.calCode,
                    companyCode = calDto.companyCode,
                    calDescription = calDto.calDescription,
                    payCode = calDto.payCode,
                    payCategory = calDto.payCategory,
                    calFormula = calDto.calFormula,
                    sequence = calDto.sequence,
                    status = true,
                    createdBy = calDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_cal);
                _context.SaveChanges();

                return _msg;
            }
            else if (calDto.flag == 'U')
            {
                var _cal = _context.Calculation.FirstOrDefault(o => o.id == calDto.id);
                if (_cal != null)
                {
                    _cal.calCode = calDto.calCode;
                    _cal.calDescription = calDto.calDescription;
                    _cal.payCategory = calDto.payCategory;
                    _cal.payCode = calDto.payCode;
                    _cal.calFormula = calDto.calFormula;
                    _cal.status = calDto.status;
                    _cal.sequence = calDto.sequence;
                    _cal.lastUpdateBy = calDto.lastUpdateBy;
                    _cal.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Calculation updated Successfully";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Calculation Formula Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else if (calDto.flag == 'D')
            {
                // Not Implemented
                return _msg;
            }
            else
            {
                return _msg;
            }
        }
        public MsgDto ManageSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = new MsgDto();

            if (specialRateEmpDto.flag == 'N')
            {
                var _sRateEmp = new EmpSpecialRate
                {
                    epf = specialRateEmpDto.epf,
                    companyCode = specialRateEmpDto.companyCode,
                    costcenter = specialRateEmpDto.costcenter,
                    payCode = specialRateEmpDto.payCode,
                    calCode = specialRateEmpDto.calCode,
                    rate = specialRateEmpDto.rate,
                    status = true,
                    createdBy = specialRateEmpDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_sRateEmp);
                _context.SaveChanges();

                _msg.MsgCode = 'S';
                _msg.Message = "Special Rate apply to Paycode " + specialRateEmpDto.payCode + " for Employee : " + specialRateEmpDto.epf;

                return _msg;
            }
            else if (specialRateEmpDto.flag == 'U')
            {
                var _sRateEmp = _context.EmpSpecialRate.FirstOrDefault(o => o.id == specialRateEmpDto.id);
                if (_sRateEmp != null)
                {
                    _sRateEmp.companyCode = specialRateEmpDto.companyCode;
                    _sRateEmp.costcenter = specialRateEmpDto.costcenter;
                    _sRateEmp.payCode = specialRateEmpDto.payCode;
                    _sRateEmp.calCode = specialRateEmpDto.calCode;
                    _sRateEmp.rate = specialRateEmpDto.rate;
                    _sRateEmp.status = specialRateEmpDto.status;
                    _sRateEmp.lastUpdateBy = specialRateEmpDto.lastUpdateBy;
                    _sRateEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Employee : " + specialRateEmpDto.epf + " Updated";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else if (specialRateEmpDto.flag == 'D')
            {
                var _sRateEmp = _context.EmpSpecialRate.FirstOrDefault(o => o.id == specialRateEmpDto.id);
                if (_sRateEmp != null)
                {
                    _sRateEmp.status = false;
                    _sRateEmp.lastUpdateBy = specialRateEmpDto.lastUpdateBy;
                    _sRateEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "1 Record Mark for Deletion";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else
            {
                return _msg;
            }
        }
        public MsgDto ManageSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = new MsgDto();

            if (specialTaxEmpDto.flag == 'N')
            {
                var _sTaxEmp = new Special_Tax_Emp
                {
                    epf = specialTaxEmpDto.epf,
                    companyCode = specialTaxEmpDto.companyCode,
                    costcenter = specialTaxEmpDto.costcenter,
                    calFormula = specialTaxEmpDto.calFormula,
                    status = true,
                    createdBy = specialTaxEmpDto.createdBy,
                    createdDate = DateTime.Now
                };

               // _context.Add(_sTaxEmp);
              //  _context.SaveChanges();

                _msg.MsgCode = 'S';
                _msg.Message = "Special Tax Rate apply to Employee : " + specialTaxEmpDto.epf;

                return _msg;
            }
            else if (specialTaxEmpDto.flag == 'U')
            {
                var _sTaxEmp = _context.Special_Tax_Emp.FirstOrDefault(o => o.id == specialTaxEmpDto.id);
                if (_sTaxEmp != null)
                {
                    _sTaxEmp.companyCode = specialTaxEmpDto.companyCode;
                    _sTaxEmp.costcenter = specialTaxEmpDto.costcenter;
                    _sTaxEmp.calFormula = specialTaxEmpDto.calFormula;
                    _sTaxEmp.status = specialTaxEmpDto.status;
                    _sTaxEmp.lastUpdateBy = specialTaxEmpDto.lastUpdateBy;
                    _sTaxEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Employee : " + specialTaxEmpDto.epf + " Updated";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else if (specialTaxEmpDto.flag == 'D')
            {
                var _sTaxEmp = _context.Special_Tax_Emp.FirstOrDefault(o => o.id == specialTaxEmpDto.id);
                if (_sTaxEmp != null)
                {
                    _sTaxEmp.status = false;
                    _sTaxEmp.lastUpdateBy = specialTaxEmpDto.lastUpdateBy;
                    _sTaxEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "1 Record Mark for Deletion";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";
                }

                _context.SaveChanges();
                return _msg;
            }
            else
            {
                return _msg;
            }
        }
    }
}
