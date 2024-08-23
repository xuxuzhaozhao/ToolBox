using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaoKenWeekLog
{
    internal class AiGenerator
    {
        private static readonly string Uri = ConfigurationManager.AppSettings["Uri"];
        private static readonly string Token = ConfigurationManager.AppSettings["Token"];
        private static readonly string Model = ConfigurationManager.AppSettings["Model"];

        public static async Task<string> SendChatGPTRequest(string prompt)
        {
            var random = new Random();
            var randomNumber = random.Next(11, 15);
            prompt = $"根据以下内容，无论我给多给少都只帮我总结{randomNumber}条周报出来，每条结论内不用换行，每条之间只换一行（请按照以下格式回答我：数字、总结的内容：总结的详情；）：{prompt}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

            var requestBody = new
            {
                model = Model, // 选择你要使用的模型
                messages = new[]
                {
                    new { role = "system", content = "换行符使用‘\r\n’" },
                    new { role = "user", content = prompt }
                }
            };

            var jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Uri, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // 解析JSON字符串
            var jsonObject = JObject.Parse(responseString);

            // 获取content的值
            string result = (string)jsonObject["choices"][0]["message"]["content"];

            return result.Replace("\n\n","\n");
        }
    }
}
