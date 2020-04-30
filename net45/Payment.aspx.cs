using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace RazorpaySampleApp
{
    public partial class Payment : System.Web.UI.Page
    {
        public string orderId;
        public string key = "rzp_live_Nd4cMOpB2WTE5x"; // your key ID
        protected void Page_Load(object sender, EventArgs e)
        {

            string secret = "wQX7T4vyT6vFRzWO9byy3kYa";

            //your order creation request

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", 100); // this amount should be same as transaction amount 
            input.Add("currency", "INR");
            input.Add("receipt", "12121");
            input.Add("payment_capture", 1);
            
            //Uncomment the below lines to enable TPV
            /*
            Dictionary<string, object> bank_account = new Dictionary<string, object>();  
            bank_account.Add("account_number", "765432123456789"); 
            bank_account.Add("name", "Gaurav");
            bank_account.Add("ifsc", "HDFC0000053"); 
            
            input.Add("method","netbanking");
            input.Add("bank_account",bank_account); */

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api-tlsv1.razorpay.com/v1/orders");
                request.Method = "POST";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                string authString = string.Format("{0}:{1}", key, secret);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

                var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(input));
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var orderResponse = (HttpWebResponse)request.GetResponse();
                orderResponse = (HttpWebResponse)request.GetResponse();

                JObject orderData = ParseResponse(orderResponse);
                orderId = orderData["id"].ToString();
            } catch (WebException ex)
            {

                using (WebResponse response = ex.Response)
                {

                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    {

                        string ErrorText = new StreamReader(data).ReadToEnd();
                        Debug.WriteLine(ErrorText);
                    }
                }
            }
        }

        private JObject ParseResponse(HttpWebResponse response)
        {
            string responseValue = string.Empty;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }

            JObject responseObject = JObject.Parse(responseValue);

            return responseObject;
        }

    }
}
