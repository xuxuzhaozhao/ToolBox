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
        private static readonly string Prompt = ConfigurationManager.AppSettings["Prompt"];

        public static async Task<string> SendChatGPTRequest(string prompt)
        {
            prompt = $"{Prompt}{prompt}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

            var requestBody = new
            {
                model = Model, // 选择你要使用的模型
                messages = new[]
                {
                    new { role = "system", content = "" },
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
