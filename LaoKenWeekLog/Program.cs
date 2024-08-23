using System.Text;
using LibGit2Sharp;
using System.Text.RegularExpressions;
using TextCopy;

namespace LaoKenWeekLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var repoPathList = new List<string>
            {
                @"C:\workspace\LK.CSSD.V2",
                @"C:\workspace\LK.CSSD.V2.Fnd",
                @"C:\workspace\LK.CSSD.V2.Fnd.CP"
            };

            var commitMessages = new List<string>();
            foreach (var repoPath in repoPathList)
            {
                using var repo = new Repository(repoPath);
                var temp = repo.Commits
                     .Where(t => !t.Message.Contains("Merge branch"))
                     .Where(t => !t.Message.Contains("cherry pick"))
                     .Where(t => t.Author.Name == "Xu.ChengYi")
                     .Where(t => t.Committer.When > DateTimeOffset.Now.AddDays(-7))
                     .SelectMany(t => t.Message.Split("\n"))
                     .ToList();
                commitMessages.AddRange(temp.Where(t => !string.IsNullOrWhiteSpace(t.Trim())));
            }

            var result = new StringBuilder();
            var index = 1;
            foreach (var commitMessage in commitMessages)
            {
                var message = new Regex(@"\b(fix|feat|report)\b|(\:|\：)|(\d+)、")
                    .Replace(commitMessage, "")
                    .Trim();
                if (string.IsNullOrWhiteSpace(message)) continue;

                result.Append($"{index}、{message}");
                //Console.WriteLine($"{index}、{message}");
                index++;
            }

            Console.WriteLine("正在生成......");

            var responseResult = AiGenerator.SendChatGPTRequest(result.ToString()).Result;
            Console.WriteLine(responseResult);

            var clipboard = new Clipboard();
            clipboard.SetText(responseResult);

            Console.WriteLine("本周周报内容填充完毕；");
            Console.ReadLine();
        }
    }
}
