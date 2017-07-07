using System;
using System.Net;
//Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.21.301221612
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Text;
//Install-Package Newtonsoft.Json 
using Newtonsoft.Json;

namespace ConsoleApplication39
{

    class Program
    {

        //Step 1 - Replace {client id} with your client app ID. 
        //To learn how to get a client app ID, see Register a client app (https://msdn.microsoft.com/en-US/library/dn877542.aspx#clientID)
        private static string clientID = "49df1bc7-db68-4fb4-91c0-6d93f770d1a4";

        //RedirectUri you used when you registered your app.
        //For a client app, a redirect uri gives AAD more details on the specific application that it will authenticate.
        private static string redirectUri = "https://login.live.com/oauth20_desktop.srf";

        //Resource Uri for Power BI API
        private static string resourceUri = "https://analysis.windows.net/powerbi/api";

        //OAuth2 authority Uri
        private static string authority = "https://login.windows.net/common/oauth2/authorize";

        private static AuthenticationContext authContext = null;
        private static string token = String.Empty;
          
        static void Main(string[] args)
        {

            
            string responseText;
            string gatewayID="";

            //get gateways
            responseText = getGateways();

            dynamic respJson =  JsonConvert.DeserializeObject<dynamic>(responseText);

            foreach (var gateway in respJson.value) { 
                Console.WriteLine(gateway["id"]); 
                Console.WriteLine(gateway["name"]);

                //get the gatewayID of my target gateway
                if (gateway["name"] == "myTargetGateway") {

                    gatewayID = gateway["id"];
                }

            }

            //get datasources from the targetGateway
            if (!string.IsNullOrEmpty(gatewayID))
            {
                responseText = getDataSources(gatewayID);


            }










            Console.ReadKey();

        }

        public static string getGateways()
        {
            string responseStatusCode = string.Empty;
              
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.powerbi.com/v1.0/myorg/gateways");

            request.Method = "GET";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken()));

            HttpWebResponse response2 = request.GetResponse() as System.Net.HttpWebResponse;

            string responseText = "bad request";

            using (HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
            {

                responseStatusCode = response.StatusCode.ToString();

                WebHeaderCollection header = response.Headers;

                var encoding = ASCIIEncoding.ASCII;

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
            }

            return responseText;
        }
        public static string getDataSources(string gatewayID)
        {
            string responseStatusCode = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://api.powerbi.com/v1.0/myorg/gateways/{0}/dataSources", gatewayID));

            request.Method = "GET";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken()));

            HttpWebResponse response2 = request.GetResponse() as System.Net.HttpWebResponse;

            string responseText = "bad request";

            using (HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
            {

                responseStatusCode = response.StatusCode.ToString();

                WebHeaderCollection header = response.Headers;

                var encoding = ASCIIEncoding.ASCII;

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
            }

            return responseText;
        }


        static string AccessToken()
        {
            if (token == String.Empty)
            {
                //Get Azure access token
                // Create an instance of TokenCache to cache the access token
                TokenCache TC = new TokenCache();
                // Create an instance of AuthenticationContext to acquire an Azure access token
                authContext = new AuthenticationContext(authority, TC);
                // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
                token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri), PromptBehavior.RefreshSession).AccessToken;
            }
            else
            {
                // Get the token in the cache
                token = authContext.AcquireTokenSilent(resourceUri, clientID).AccessToken;
            }

            return token;
        }
    }
}
