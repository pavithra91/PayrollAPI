using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using SAP.Middleware.Connector;

namespace PayrollAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTransferController : ControllerBase
    {
        private readonly IDatatransfer _data;
        public DataTransferController(IDatatransfer data)
        {
            _data = data;
        }

        [Route("TestRFC")]
        [HttpPost]
        public IActionResult TestRFC()
        {
            SAPConnect sap = new SAPConnect();
            RfcDestinationManager.RegisterDestinationConfiguration(sap);
            RfcDestination prd = RfcDestinationManager.GetDestination("ZRFC_TEST");

            try
            {
                RfcRepository repo = prd.Repository;
                IRfcFunction companyBapi = repo.CreateFunction("BAPI_COMPANY_GETDETAIL");
                companyBapi.SetValue("COMPANYID", "001000");
                companyBapi.Invoke(prd);
                IRfcStructure detail = companyBapi.GetStructure("COMPANY_DETAIL");
                String companyName = detail.GetString("NAME1");
                Console.WriteLine(companyName);

                return Ok();
            }
            catch (RfcCommunicationException e)
            {
                return BadRequest(e);
            }
            catch (RfcLogonException e)
            {
                return BadRequest(e);
            }
            catch (RfcAbapRuntimeException e)
            {
                return BadRequest(e);
            }
            catch (RfcAbapBaseException e)
            {
                return BadRequest(e);
            }

        }


        [Route("ConfirmDataTransfer")]
        [HttpPost]
        public async Task<IActionResult> ConfirmDataTransfer([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _data.ConfirmDataTransfer(approvalDto);


            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("PreparePayrun")]
        [HttpPost]
        public async Task<IActionResult> PreparePayrun([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _data.PreparePayrun(approvalDto);


            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }
    }
}
