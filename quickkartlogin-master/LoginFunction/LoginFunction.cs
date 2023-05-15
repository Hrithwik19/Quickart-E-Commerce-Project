using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace LoginFunction
{
    public static class LoginFunction
    {
        [FunctionName("LoginFunction")]
        public static async Task<int> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Streaming the body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            //Converting to JSON
            dynamic data = JsonConvert.DeserializeObject(requestBody);


            string userEmailID = data.emailID;
            string userPassword = data.password;
            string userType=data.usertype;
            int result = 0;

            //SQL connection
            SqlConnection conObj = new SqlConnection("Data Source=quickcart-server.database.windows.net;Initial Catalog=QuickCart-DB;user id=demouser; password=Siddharth@1234");
            
            //command
            SqlCommand cmdObj = new SqlCommand("select [dbo].ufn_ValidateLogin(@userEmailID,@userPassword,@customerType)", conObj);

            cmdObj.Parameters.AddWithValue("@userEmailID", userEmailID);
            cmdObj.Parameters.AddWithValue("@userPassword", userPassword);
            cmdObj.Parameters.AddWithValue("@customerType", userType);

            try
            {
                conObj.Open();
                result = Convert.ToInt32(cmdObj.ExecuteScalar());


             }
            catch (Exception e)
            {
                result = -1;
                log.LogInformation("Exception Occured\n\n" + e.Message + "\n\n");
            }
            finally
            {
                conObj.Close();
            }



            return result;
        }
    }
}
