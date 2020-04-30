using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RazorpaySampleApp
{
    public partial class Charge : System.Web.UI.Page
    {
        public string paymentId;
        public string paymentStatus;


        protected void Page_Load(object sender, EventArgs e)
        {

            string key = "API_Key";
            string secret = "API_Secret";

            string receivedSignature = Request.Form["razorpay_signature"];
            string orderId = Request.Form["razorpay_order_id"];
            paymentId = Request.Form["razorpay_payment_id"];
            string payload = string.Format("{0}|{1}", orderId, paymentId);

            if(verifySignature(payload, receivedSignature, secret))
            {
                //Show your thank you page
            }

            //As an additional step , you can fecth the payment status to verfy the same as well to get all the payment data back

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.razorpay.com/v1/payments/" + paymentId);
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                string authString = string.Format("{0}:{1}", key, secret);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

                var paymentResponse = (HttpWebResponse)request.GetResponse();

                JObject paymentStatusData = ParseResponse(paymentResponse);
                paymentStatus = paymentStatusData["status"].ToString();

                Debug.WriteLine(paymentStatus);


            } catch(WebException ex){

                using (WebResponse response = ex.Response) {

                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream()){

                        string ErrorText = new StreamReader(data).ReadToEnd();
                        Debug.WriteLine(ErrorText);
                    }
                }
            }


        }

        private bool verifySignature(string payload,string receivedSignature, string secret)
        {
            byte[] secretBytes = StringEncode(secret);
            HMACSHA256 hashHmac = new HMACSHA256(secretBytes);
            var bytes = StringEncode(payload);
            string actualSignature = HashEncode(hashHmac.ComputeHash(bytes));
            Debug.WriteLine(actualSignature);
            bool verified = actualSignature.Equals(receivedSignature);
            return verified;
        }


        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
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
