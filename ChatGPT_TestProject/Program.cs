using System.Net;
using Newtonsoft.Json;
using Flurl.Http;
using System.Text;

namespace ChatGPT_TestProject
{
    internal class Program
    {
        private static readonly string apiKey = "sk-uH2ZfAZUDRwLzuIlC5496fDd930743A8Ba36A04c208d4217"; // 替换为你的API密钥
        private static readonly string apiUrl = "http://110.40.41.167:21531/v1/chat/completions";

        static async Task Main(string[] args)
        {
            string test = @"根据以下内容，请按照以下格式回答我：1、XXXXX...：XXXXXXXXXXXXXXXX...；，帮我总结12条周报出来: 1、修复【CP端】【Admin端】请确认数据组禁用，仍然可进行权限控制；
2、修复【CP端】【Admin端】组件列表筛选项“ 是否关联包模板”选择“已关联”，查询结果有误；
3、CP端若没有该外来器械商的权限，则不能对该处理单进行编辑操作，点击编辑按钮后提示“该账号暂无编辑外来器械商XXX所属处理单的权限！”；
4、修改提示信息翻译；
5、修复外来器械组件名称应该根据外来器械商+组件名称+规格进行判断；
6、修复【Admin端】新增外来器械处理单，外来器械商显示到“医院”的枚举项中了；
7、修复【管理端】【CP端】删除包模板之后，该包模板下的组件状态应该更新；
8、修复【CP端】新增包模板页面选择“自定义组件”但是未添加组件进行保存，该模板的详情页面显示有误；
9、修复【Admin端】外来器械组件列表存在完全相同的两条数据；
10、菜单名称由发货单详情改为送货单详情;
11、修复【CP端】某外来器械商访问级别配置为“公共的”，创建外来器械处理单时，不能被选择；
12、修复机器批次详情表头各个筛选条件；
13、修复【3.0】【Admin端】外来器械单选择包模板-查看详情弹窗中“服务需求”与该包模板详情页面“服务需求”不一致；
14、pdmaner 将包模板字段CustomerServiceRequirementId修改为CustomerServiceRequirementDefinitionId；
15、后端推荐分包数已修复为可为空;
16、完成【Admin端】外来器械商列表未增加显示“数据组”字段；
17、分组客户不需要展示默认客户；
18、获取分组后的客户信息排除掉默认客户（中心）；
19、玩车获取当前用户可查看的所有分组后的客户接口；
20、添加获取当前用户可查看的所有分组后的客户接口；
21、CP端创建外来器械单时获取外来器械商增加数据组权限；
22、添加外来器械组件名称+规格；
23、完成外来器械组件修改为名称+规格验证是否重名；
24、添加消息翻译文件；
25、补充3.0.4版本工作系数、可拆卸组件、高值组件、客户权重字段脚本；
26、(cherry picked from commit 2d481aeb7d55ee9509eade396f0f61a5329ff5ae)
27、添加字段注释；
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
