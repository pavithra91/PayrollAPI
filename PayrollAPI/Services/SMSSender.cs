using Newtonsoft.Json;
using System.ServiceModel.Channels;
using System.Text;

namespace PayrollAPI.Services
{
    public class SMSSender
    {
        public string _receiver { get; set; }
        public string _message { get; set; }
        public string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/";

        public SMSSender(string receiver, string message)
        {
            _receiver = receiver;
            _message = message;
        }
        public async Task<bool> sendSMS(SMSSender sms)
        {
            using (var client = new HttpClient())
            {
                string url = "https://msmsenterpriseapi.mobitel.lk/EnterpriseSMSV3/esmsproxyURL.php";

                // Create a dictionary to hold the request data
                var requestData = new Dictionary<string, string>()
                {
                    { "username", "esmsusr_s4u" },
                    { "password", "600aon" },
                    { "from", "CPSTL" },
                    { "text", sms._message },
                    { "to", sms._receiver },
                    { "messageType", "0" }
                };

                string jsonContent = JsonConvert.SerializeObject(requestData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}
