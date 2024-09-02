using System.Net;
using Newtonsoft.Json;
using Flurl.Http;
using System.Text;

namespace ChatGPT_TestProject
{
    internal class Program
    {
        private static readonly string apiKey = "sk-uH2ZfAZUDRwLzuIlC5496fDd930743A8Ba36A04c208d4218"; // 替换为你的API密钥
        private static readonly string apiUrl = "http://110.40.41.167:21531/v1/chat/completions";

        static async Task Main(string[] args)
        {
            string test = @"
";

            var response = await SendChatGPTRequest(test);
            Console.WriteLine(response);
        }

        static async Task<string> SendChatGPTRequest(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "deepseek-chat", // 选择你要使用的模型
                messages = new[]
                {
                    new { role = "system", content = "" },
                    new { role = "user", content = prompt }
                }
            };

            var jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
