using Newtonsoft.Json;
using System.Text;

namespace PayrollAPI.Data
{
    public class SMSSender
    {
        public string _receiver { get; set; }
        public string _message { get; set; }
        public string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/";

        public SMSSender(string receiver, string message)
        {
            this._receiver = receiver;
            this._message = message;
        }
        public async void sendSMS(SMSSender sms)
        {
            using (var client = new HttpClient())
            {
                string url = "https://msmsenterpriseapi.mobitel.lk/mSMSEnterpriseAPI/esmsproxy.php?u=esmsusr_s4u&p=600aon&a=CPSTL&m=" + sms._message + "&r=" + sms._receiver + "&t=0";
                var request = await client.GetAsync(url);
                var response = await request.Content.ReadAsStringAsync();

                Console.WriteLine(response);
            }

        }
    }
}
