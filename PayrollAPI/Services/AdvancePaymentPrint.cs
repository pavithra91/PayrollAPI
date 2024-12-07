using ClosedXML.Excel;
using PayrollAPI.Models;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Services
{
    public class AdvancePaymentPrint
    {
        public byte[] GenerateAdvancePaymentReport(IEnumerable<AdvancePayment> advancePayment, int period)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(period);

            // Adding headers
            worksheet.Cell(1, 1).Value = "EPF";
            worksheet.Cell(1, 2).Value = "Employee Name";
            worksheet.Cell(1, 3).Value = "Period";
            worksheet.Cell(1, 4).Value = "Full Amount";
            worksheet.Cell(1, 5).Value = "Amount";
            worksheet.Cell(1, 6).Value = "Request Date";

            int row = 2;
            foreach (var payment in advancePayment)
            {
                worksheet.Cell(row, 1).Value = payment.employee.epf;
                worksheet.Cell(row, 2).Value = payment.employee.empName;
                worksheet.Cell(row, 3).Value = payment.period;
                worksheet.Cell(row, 4).Value = payment.isFullAmount;
                worksheet.Cell(row, 5).Value = payment.amount;
                worksheet.Cell(row, 6).Value = payment.createdDate;
                row++;
            }

            // Save to memory stream
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
            //return memoryStream;
        }
    }
}
