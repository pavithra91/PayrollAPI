namespace PayrollAPI.Services
{
    public class Common
    {
        public async Task UploadPdfToS3(byte[] pdfData, string fileName, string period)
        {
            try
            {
                var uploader = new S3Uploader("AKIAV3CJE2DCBB7UZJDM", "oPvNVvN3U5e+MZwtmRK8/X+5kLDxNzXsCubr1XbT", "cpstl-poc-main-s3", Amazon.RegionEndpoint.APSoutheast1);
                await uploader.UploadFileAsync(pdfData, "public/" + period + "/" + fileName);

                Thread.Sleep(300);
            }
            catch (Exception ex)
            {

            }
        }
        public string GetPeriod(string period)
        {
            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));
            DateTime date = new DateTime(year, month, 1);

            // Format the DateTime as "YYYY MMMM"
            return date.ToString("MMM yyyy").ToUpper();
        }

        public DateTime GetTimeZone()
        {
            string userTimeZoneId = "Asia/Colombo";
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);

            DateTime utcDate = DateTime.UtcNow;
            DateTime userLocalDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, userTimeZone);

            return userLocalDate;
        }
    }
}
