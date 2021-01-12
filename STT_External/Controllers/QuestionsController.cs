using Dapper;
using Newtonsoft.Json;
using STT_External.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace STT_External.Controllers
{
    public class QuestionsController : ApiController
    {
        // POST api/Questions
        public HttpResponseMessage Post(Question question)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var output = new List<Answer>();
            var optimalDistance = 5;
            HttpResponseMessage result;
            using (IDbConnection cnn = new SQLiteConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("QuestionId", question.Id);
                output = cnn.Query<Answer>("select * from Answer where QuestionId = :QuestionId", dynamicParameters).ToList();
            }
            var payload = new Payload()
            {
                Word = question.Description,
                ExpectedWords = output,
                Distance = optimalDistance
            };
            string jsonObject = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://localhost:44374/");
                result = client.PostAsync("/api/Match/", content).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;

                //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                //Payload match = json_serializer.Deserialize<Payload>(resultContent);
            }

            return result;
        }
    }
}
