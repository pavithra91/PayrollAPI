using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NReco.PivotData;
using Org.BouncyCastle.Ocsp;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using PdfSharp.Charting;
using PdfSharp.Pdf.Content.Objects;
using System.Data;
using static LinqToDB.Common.Configuration;

namespace PayrollAPI.Repository
{
    public class AdminRepository : IAdmin
    {
        private readonly DBConnect _context;
        public AdminRepository(DBConnect context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> GetTaxDetails()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _taxList = await _context.Tax_Calculation.Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.range,
                    o.calFormula,
                    o.description,
                    o.taxCategory,
                    o.contributor,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_taxList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_taxList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetTaxDetailsById(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _tax = await _context.Tax_Calculation.Where(o => o.id == id).Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.range,
                    o.calFormula,
                    o.description,
                    o.taxCategory,
                    o.contributor,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_tax != null)
                {
                    _msg.Data = JsonConvert.SerializeObject(_tax);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreateTaxCalculation(TaxCalDto taxCalDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _tax = new Tax_Calculation
                {
                    companyCode = taxCalDto.companyCode,
                    calFormula = taxCalDto.calFormula,
                    description = taxCalDto.description,
                    taxCategory = taxCalDto.taxCategory,
                    range = taxCalDto.range,
                    status = true,
                    createdBy = taxCalDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_tax);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Tax Calculation Created Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> UpdateTax(TaxCalDto taxCalDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _tax = _context.Tax_Calculation.FirstOrDefault(o => o.id == taxCalDto.id);
                if (_tax != null)
                {
                    _tax.calFormula = taxCalDto.calFormula ?? _tax.calFormula;
                    _tax.description = taxCalDto.description ?? _tax.description;
                    _tax.taxCategory = taxCalDto.taxCategory ?? _tax.taxCategory;

                    if (taxCalDto.range > 0)
                    {
                        _tax.range = taxCalDto.range;
                    }

                    if (_tax.status != taxCalDto.status)
                    {
                        _tax.status = taxCalDto.status;
                    }

                    _tax.lastUpdateBy = taxCalDto.lastUpdateBy;
                    _tax.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Tax updated Successfully";

                    _context.Entry(_tax).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Tax Code Found";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetPayCodes()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _payCodeList = await _context.PayCode.Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.description,
                    o.payCategory,
                    o.rate,
                    o.taxationType,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_payCodeList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_payCodeList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetPayCodesById(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _payCode = await _context.PayCode.Where(o => o.id == id).Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.description,
                    o.payCategory,
                    o.rate,
                    o.taxationType,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_payCode.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_payCode);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreatePayCode(PayCodeDto payCodeDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _ePayCode = _context.PayCode.Where(o => o.payCode == payCodeDto.payCode && o.companyCode == payCodeDto.companyCode).FirstOrDefault();

                if (_ePayCode != null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Pay Code already exists";
                    return _msg;
                }

                if (payCodeDto.payCode == 0 || payCodeDto.companyCode == 0)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Please enter Valid Pay Code";
                    return _msg;
                }

                var _payCode = new PayCode
                {
                    calCode = payCodeDto.calCode,
                    companyCode = payCodeDto.companyCode,
                    description = payCodeDto.description,
                    payCode = payCodeDto.payCode,
                    payCategory = payCodeDto.payCategory,
                    rate = payCodeDto.rate,
                    taxationType = payCodeDto.taxationType,
                    createdBy = payCodeDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_payCode);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Pay Code Created Successfully";

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }

        }
        public async Task<MsgDto> UpdatePayCode(PayCodeDto payCodeDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _payCode = _context.PayCode.FirstOrDefault(o => o.id == payCodeDto.id);
                if (_payCode != null)
                {
                    _payCode.calCode = payCodeDto.calCode ?? _payCode.calCode;
                    _payCode.description = payCodeDto.description ?? _payCode.description;
                    _payCode.payCategory = payCodeDto.payCategory ?? _payCode.payCategory;
                    _payCode.taxationType = payCodeDto.taxationType ?? _payCode.taxationType;

                    //if (_payCode.isTaxableGross != payCodeDto.isTaxableGross)
                    //{
                    //    _payCode.isTaxableGross = payCodeDto.isTaxableGross;
                    //}

                    if (payCodeDto.rate >= 0)
                    {
                        _payCode.rate = payCodeDto.rate;
                    }

                    _payCode.lastUpdateBy = payCodeDto.lastUpdateBy;
                    _payCode.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Pay Code updated Successfully";

                    _context.Entry(_payCode).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Pay Code Found";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetCalculations()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _calculationList = await _context.Calculation.Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.calFormula,
                    o.calDescription,
                    o.payCategory,
                    o.contributor,
                    o.sequence,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_calculationList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_calculationList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetCalculationsById(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _calculation = await _context.Calculation.Where(o => o.id == id).Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.calFormula,
                    o.calDescription,
                    o.payCategory,
                    o.contributor,
                    o.sequence,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_calculation.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_calculation);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreateCalculation(CalDto calDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _eCal = _context.Calculation.FirstOrDefault(o => o.calCode == calDto.calCode);

                if (_eCal != null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Cal Code already exists";
                    return _msg;
                }

                if (calDto.companyCode == 0 || calDto.sequence == 0 || calDto.calFormula == null || calDto.calCode == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Payload have Null Values";
                    return _msg;
                }

                var _cal = new Calculation
                {
                    calCode = calDto.calCode,
                    companyCode = calDto.companyCode,
                    calDescription = calDto.calDescription,
                    payCode = calDto.payCode,
                    payCategory = calDto.payCategory,
                    contributor = calDto.contributor,
                    calFormula = calDto.calFormula,
                    sequence = calDto.sequence,
                    status = true,
                    createdBy = calDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_cal);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Calculation Created Successfully";

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> UpdateCalculation(CalDto calDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _cal = _context.Calculation.FirstOrDefault(o => o.id == calDto.id);
                if (_cal != null)
                {
                    _cal.calCode = calDto.calCode ?? _cal.calCode;
                    _cal.calDescription = calDto.calDescription ?? _cal.calDescription;
                    _cal.payCategory = calDto.payCategory ?? _cal.payCategory;
                    _cal.calFormula = calDto.calFormula ?? _cal.calFormula;
                    _cal.contributor = calDto.contributor ?? _cal.contributor;

                    if (_cal.payCode != calDto.payCode)
                    {
                        _cal.payCode = calDto.payCode;
                    }
                    if (_cal.sequence != calDto.sequence)
                    {
                        _cal.sequence = calDto.sequence;
                    }
                    _cal.lastUpdateBy = calDto.lastUpdateBy;
                    _cal.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Calculation updated Successfully";
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Calculation Formula Found";
                    return _msg;
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> DeleteCalculation(CalDto calDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _cal = _context.Calculation.FirstOrDefault(o => o.id == calDto.id);
                if (_cal != null)
                {
                    _cal.status = false;
                    _cal.lastUpdateBy = calDto.lastUpdateBy;
                    _cal.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Calculation Mark for Deletion";
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Calculation Formula Found";
                    return _msg;
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetSplRateEmp()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _empSplRateList = await _context.EmpSpecialRate.Select(o => new
                {
                    o.id,
                    o.epf,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.costCenter,
                    o.location,
                    o.rate,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_empSplRateList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_empSplRateList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetSplRateEmpById(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _empSplRate = await _context.EmpSpecialRate.Where(o => o.id == id).Select(o => new
                {
                    o.id,
                    o.epf,
                    o.companyCode,
                    o.payCode,
                    o.calCode,
                    o.costCenter,
                    o.location,
                    o.rate,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_empSplRate != null)
                {
                    _msg.Data = JsonConvert.SerializeObject(_empSplRate);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreateSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _eSplRate = _context.EmpSpecialRate.FirstOrDefault(o => o.payCode == specialRateEmpDto.payCode && o.epf == specialRateEmpDto.epf && o.status == true);

                if (_eSplRate != null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Active Special Rate Already assign to Employee : " + specialRateEmpDto.epf + " for Paycode : " + specialRateEmpDto.payCode;
                }

                var _sRateEmp = new EmpSpecialRate
                {
                    epf = specialRateEmpDto.epf,
                    companyCode = specialRateEmpDto.companyCode,
                    costCenter = specialRateEmpDto.costCenter,
                    payCode = specialRateEmpDto.payCode,
                    calCode = specialRateEmpDto.calCode,
                    rate = specialRateEmpDto.rate,
                    status = true,
                    createdBy = specialRateEmpDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_sRateEmp);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Special Rate apply to Paycode " + specialRateEmpDto.payCode + " for Employee : " + specialRateEmpDto.epf;

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> UpdateSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _sRateEmp = _context.EmpSpecialRate.FirstOrDefault(o => o.id == specialRateEmpDto.id);
                if (_sRateEmp != null)
                {
                    _sRateEmp.costCenter = specialRateEmpDto.costCenter ?? _sRateEmp.costCenter;
                    _sRateEmp.calCode = specialRateEmpDto.calCode ?? _sRateEmp.calCode;

                    if (_sRateEmp.payCode != specialRateEmpDto.payCode)
                    {
                        _sRateEmp.payCode = specialRateEmpDto.payCode;
                    }
                    if (_sRateEmp.rate != specialRateEmpDto.rate)
                    {
                        _sRateEmp.rate = specialRateEmpDto.rate;
                    }

                    _sRateEmp.lastUpdateBy = specialRateEmpDto.lastUpdateBy;
                    _sRateEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Employee : " + specialRateEmpDto.epf + " Updated";
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Record Found";
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> DeleteSpecialRateEmp(SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
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
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Record Found";
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetSplTaxEmp()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _empSplTaxList = await _context.Special_Tax_Emp.Select(o => new
                {
                    o.id,
                    o.epf,
                    o.companyCode,
                    o.costCenter,
                    o.location,
                    o.calFormula,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_empSplTaxList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_empSplTaxList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetSplTaxEmpById(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _empSplTax = await _context.Special_Tax_Emp.Where(o => o.id == id).Select(o => new
                {
                    o.id,
                    o.epf,
                    o.companyCode,
                    o.costCenter,
                    o.location,
                    o.calFormula,
                    o.status,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_empSplTax != null)
                {
                    _msg.Data = JsonConvert.SerializeObject(_empSplTax);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreateSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _sTaxEmp = new Special_Tax_Emp
                {
                    epf = specialTaxEmpDto.epf,
                    companyCode = specialTaxEmpDto.companyCode,
                    costCenter = specialTaxEmpDto.costCenter,
                    calFormula = specialTaxEmpDto.calFormula,
                    status = true,
                    createdBy = specialTaxEmpDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Special_Tax_Emp.Add(_sTaxEmp);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Special Tax Rate apply to Employee : " + specialTaxEmpDto.epf;

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> UpdateSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _sTaxEmp = _context.Special_Tax_Emp.FirstOrDefault(o => o.id == specialTaxEmpDto.id);
                if (_sTaxEmp != null)
                {
                    _sTaxEmp.costCenter = specialTaxEmpDto.costCenter ?? _sTaxEmp.costCenter;
                    _sTaxEmp.calFormula = specialTaxEmpDto.calFormula ?? _sTaxEmp.calFormula;
                    _sTaxEmp.lastUpdateBy = specialTaxEmpDto.lastUpdateBy;
                    _sTaxEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Employee : " + specialTaxEmpDto.epf + " Updated";
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Employee Found";
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> DeleteSpecialTaxEmp(SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _sTaxEmp = _context.Special_Tax_Emp.FirstOrDefault(o => o.id == specialTaxEmpDto.id);
                if (_sTaxEmp != null)
                {
                    _sTaxEmp.status = false;
                    _sTaxEmp.lastUpdateBy = specialTaxEmpDto.lastUpdateBy;
                    _sTaxEmp.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Record Mark for Deletion";
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Record Found";
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetOTDetails(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _othoursList = _context.GetOTDetails.FromSqlRaw("SELECT * FROM payrolldb.OTHours_View WHERE period= " + period + ";").ToList();
                var _summaryList = _context.GetSummaryDetails.FromSqlRaw("SELECT * FROM payrolldb.Payroll_Summary_View ORDER BY period DESC LIMIT 12;").ToList();

                _summaryList.Reverse();

                DataTable dt = new DataTable();
                dt.Columns.Add("overTimeData");
                dt.Columns.Add("summaryData");

                if (_othoursList.Count > 0)
                {
                    dt.Rows.Add(JsonConvert.SerializeObject(_othoursList), JsonConvert.SerializeObject(_summaryList));
                    _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }


        public async Task<MsgDto> GetUnrecoveredDetails(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _unrecoveredList = await _context.Unrecovered.Where(o => o.companyCode == companyCode && o.period == period).Select(o => new
                {
                    o.epf,
                    o.costCenter,
                    o.location,
                    o.payCode,
                    o.amount
                }
                ).ToListAsync();

                if (_unrecoveredList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_unrecoveredList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetLumpSumTaxDetails(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _lumpSumTaxList = await _context.EPF_ETF.Where(o => o.companyCode == companyCode && o.period == period && o.lumpSumGross > 0).Select(o => new
                {
                    o.epf,
                    o.empName,
                    o.grade,
                    o.location,
                    o.lumpSumGross,
                    o.lumpsumTax,
                }
                ).ToListAsync();

                if (_lumpSumTaxList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_lumpSumTaxList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> ResetData(ResetDto resetDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                var _payrollData = _context.Payroll_Data.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                var _empData = _context.Employee_Data.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                var _unrecoevered = _context.Unrecovered.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                var _epf = _context.EPF_ETF.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();

                if (resetDto.resetTempData)
                {
                    var _totPayCodes = _context.TotPayCode.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                    var _sapPayCodes = _context.SAPTotPayCode.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                    var _payrun = _context.Payrun.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                    var _tempEmp = _context.Temp_Employee.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                    var _tempPayroll = _context.Temp_Payroll.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).DeleteFromQuery();
                }
                else
                {
                    Payrun _payrun = _context.Payrun.Where(o => o.companyCode == resetDto.companyCode && o.period == resetDto.period).FirstOrDefault();
                    _payrun.payrunStatus = "Transfer Complete";
                    _payrun.approvedBy = string.Empty;
                    _payrun.payrunBy = string.Empty;
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetSystemVariables()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _systemVaiablesList = await _context.Sys_Properties.Select(o => new
                {
                    o.id,
                    o.companyCode,
                    o.category_name,
                    o.variable_name,
                    o.variable_value,
                    o.createdBy,
                    o.createdDate,
                    o.createdTime,
                    o.lastUpdateBy,
                    o.lastUpdateDate,
                    o.lastUpdateTime
                }).ToListAsync();

                if (_systemVaiablesList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_systemVaiablesList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> CreateSystemVariable(SysVariableDto sysVariableDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _sysVariable = new Sys_Properties
                {
                    companyCode = sysVariableDto.companyCode,
                    category_name = sysVariableDto.category_name,
                    variable_name = sysVariableDto.variable_name,
                    variable_value = sysVariableDto.variable_value,
                    createdBy = sysVariableDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_sysVariable);
                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "System Variable Created Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> UpdateSystemVariable(SysVariableDto sysVariableDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _sysVariable = _context.Sys_Properties.FirstOrDefault(o => o.id == sysVariableDto.id);
                if (_sysVariable != null)
                {
                    _sysVariable.category_name = sysVariableDto.category_name ?? _sysVariable.category_name;
                    _sysVariable.variable_name = sysVariableDto.variable_name ?? _sysVariable.variable_name;
                    _sysVariable.variable_value = sysVariableDto.variable_value ?? _sysVariable.variable_value;

                    _sysVariable.lastUpdateBy = sysVariableDto.lastUpdateBy;
                    _sysVariable.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "System Variable updated Successfully";

                    _context.Entry(_sysVariable).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = $"System Variable {sysVariableDto.variable_name} Found";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetPayCodeWiseData(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {

                ICollection<Payroll_Data> payItemList = _context.Payroll_Data.Where(x => x.companyCode == companyCode && x.period == period).OrderBy(x=>x.epf).ToList();
                ICollection<Employee_Data> emp = _context.Employee_Data.Where(x => x.companyCode == companyCode && x.period == period).OrderBy(x => x.epf).ToList();


                var _resultList = from payData in payItemList
                                           join empData in emp on payData.epf equals empData.epf
                                         into Deductions
                                           from defaultVal in Deductions.DefaultIfEmpty()
                                           orderby payData.epf
                                           select new
                                           {
                                               epf = payData.epf,
                                               empName = defaultVal.empName,
                                               payCode = payData.payCode,
                                               amount = payData.amount,
                                           };

                var _payItemList = _resultList.GroupBy(i => new { i.payCode, i.epf, i.empName }).Select(g => new
                {
                    payCode = g.Key.payCode,
                    epf = g.Key.epf,
                    empName = g.Key.empName,
                    amount = g.Sum(i => i.amount)
                });

                DataTable dt = new DataTable();
                dt.Columns.Add("epf");
                dt.Columns.Add("empName");
                dt.Columns.Add("payCode");
                dt.Columns.Add("amount");

                 foreach (var item in _payItemList) {
                    dt.Rows.Add(item.epf, item.empName, item.payCode.ToString(), item.amount);
                 }

                var _pvtData = new PivotData(new[] { "epf", "empName", "payCode", "amount" }, new SumAggregatorFactory("amount"));
                _pvtData.ProcessData(new DataTableReader(dt));

                var _pvtTbl = new PivotTable(
                    new[] { "epf", "empName" }, //rows
                    new[] { "payCode" }, //columns
                    _pvtData);

                var dataTableWr = new NReco.PivotData.Output.PivotTableDataTableWriter("PaycodeWiseData");
                dataTableWr.GrandTotal = false;
                dataTableWr.TotalsRow = false;
                dataTableWr.TotalsColumn = false;
                DataTable tbl = dataTableWr.Write(_pvtTbl);

                tbl.Columns.RemoveAt(2);
                
                tbl.Columns["epf[trial]"].ColumnName = "EPF";
                tbl.Columns["empname[trial]"].ColumnName = "Employee Name";

                _msg.Data = JsonConvert.SerializeObject(tbl);
                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
    }
}
