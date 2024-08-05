using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;

namespace LaoKenMonthLog
{
    /// <summary>
    /// TAPD 工时报表【1、导出明细报表，2、注册右键，3、生成月报】
    /// </summary>
    internal class Program
    {
        private static string SaveToPath => ConfigurationManager.AppSettings[nameof(SaveToPath)];
        private static string DevelopRole => ConfigurationManager.AppSettings[nameof(DevelopRole)];
        private static string UserName => ConfigurationManager.AppSettings[nameof(UserName)];

        static void Main(string[] args)
        {
            string filePath = string.Empty;
            if (args.Length == 1)
            {
                filePath = args[0];
                if (!filePath.Contains(".xls"))
                {
                    Console.WriteLine("请打开正确的xls文件。");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                Console.WriteLine("通过右键单击打开此文件。");
                Console.ReadKey();
                return;
            }

            var all = new Dictionary<string, double>();
            using var fsRead = new FileStream(filePath, FileMode.Open);
            //创建工作薄
            IWorkbook workBook = new HSSFWorkbook(fsRead);
            //获取Sheet
            ISheet sheet = workBook.GetSheetAt(0);

            DataRow dr;
            for (var i = 2; i < sheet.LastRowNum; i++)
            {
                //当前行数据
                var currentRow = sheet.GetRow(i);
                if (currentRow == null || currentRow.GetCell(4) == null || currentRow.GetCell(5) == null) continue;

                var story = currentRow.GetCell(4).StringCellValue;
                var hour = currentRow.GetCell(5).StringCellValue;

                AddData(all, story, hour);
            }

            foreach (var key in all.Keys)
            {
                Console.WriteLine($"{key} ===> {all[key]}");
            }

            IWorkbook workbook = new XSSFWorkbook();
            var workSheet = workbook.CreateSheet("Sheet1");
            CreateBasicExcel(workSheet, workbook, all);
            CreateHeadCell(workSheet, workbook);
            CreateBodyCell(workSheet, workbook, all);
            CreateFootCell(workSheet, workbook, all);
            SaveFile(workbook);
            Console.WriteLine("success");

            Console.ReadKey();
        }

        private static void AddData(Dictionary<string, double> all, string story, string hour)
        {
            if (!story.Contains("【Story】")) return;

            if (!double.TryParse(hour, out double hourDouble)) return;

            story = story.Split('\r', '\n')[0]
                .Replace("【Story】", "")
                .Replace("【CSSD 消毒追溯管理系统】", "");
            if (all.ContainsKey(story))
            {
                all[story] = all[story] + hourDouble;
            }
            else
            {
                all[story] = hourDouble;
            }
        }

        private static void CreateBasicExcel(ISheet sheet, IWorkbook workbook, Dictionary<string, double> dictionary)
        {
            sheet.SetColumnWidth(0, 4 * 256);
            sheet.SetColumnWidth(1, 10 * 256);
            sheet.SetColumnWidth(2, 15 * 256);
            sheet.SetColumnWidth(3, 10 * 256);
            sheet.SetColumnWidth(4, 60 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 20 * 256);

            ICellStyle basicCellStyle = sheet.Workbook.CreateCellStyle();
            basicCellStyle.BorderBottom = BorderStyle.Thin;
            basicCellStyle.BorderLeft = BorderStyle.Thin;
            basicCellStyle.BorderRight = BorderStyle.Thin;
            basicCellStyle.BorderTop = BorderStyle.Thin;

            for (int i = 1; i < (dictionary.Keys.Count + 7); i++)
            {
                var row = sheet.CreateRow(i);
                for (int j = 1; j < 7; j++)
                {
                    var cell1 = row.CreateCell(j);
                    cell1.CellStyle = basicCellStyle;
                }
            }
        }

        private static void CreateHeadCell(ISheet sheet, IWorkbook workbook)
        {
            sheet.AddMergedRegion(new CellRangeAddress(1, 3, 1, 1));
            var cell1 = sheet.GetRow(1).GetCell(1);
            cell1.SetCellValue("主序号");
            cell1.CellStyle = HeaderCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(1, 3, 2, 2));
            var cell2 = sheet.GetRow(1).GetCell(2);
            cell2.SetCellValue("开 发 项 目");
            cell2.CellStyle = HeaderCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(1, 3, 3, 3));
            var cell3 = sheet.GetRow(1).GetCell(3);
            cell3.SetCellValue("子序号");
            cell3.CellStyle = HeaderCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(1, 3, 4, 4));
            var cell4 = sheet.GetRow(1).GetCell(4);
            cell4.SetCellValue("事  项  描  述");
            cell4.CellStyle = HeaderCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(1, 3, 5, 5));
            var cell5 = sheet.GetRow(1).GetCell(5);
            cell5.SetCellValue("开发周期");
            cell5.CellStyle = HeaderCellStyle(workbook, sheet);

            var cell6 = sheet.GetRow(1).GetCell(6);
            cell6.SetCellValue("工时消耗（小时）");
            cell6.CellStyle = HeaderCellStyle(workbook, sheet);

            var cell7 = sheet.GetRow(2).GetCell(6);
            cell7.SetCellValue(DevelopRole);
            cell7.CellStyle = HeaderCellStyle(workbook, sheet);

            var cell8 = sheet.GetRow(3).GetCell(6);
            cell8.SetCellValue(UserName);
            cell8.CellStyle = AlignCenterCellStyle(workbook, sheet);
        }

        private static void CreateBodyCell(ISheet sheet, IWorkbook workbook, Dictionary<string, double> dictionary)
        {
            sheet.AddMergedRegion(new CellRangeAddress(4, dictionary.Keys.Count + 3, 1, 1));
            var cell1 = sheet.GetRow(4).GetCell(1);
            cell1.SetCellValue("1");
            cell1.CellStyle = AlignCenterCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(4, dictionary.Keys.Count + 3, 2, 2));
            var cell2 = sheet.GetRow(4).GetCell(2);
            cell2.SetCellValue("TrakOne 3.0国际版EP1");
            cell2.CellStyle = AlignCenterCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(4, dictionary.Keys.Count + 3, 5, 5));
            var cell3 = sheet.GetRow(4).GetCell(5);
            cell3.SetCellValue($"{DateTime.Now.AddMonths(-1).Month}月");
            cell3.CellStyle = AlignCenterCellStyle(workbook, sheet);

            var i = 0;
            foreach (var key in dictionary.Keys)
            {
                var cell4 = sheet.GetRow(4 + i).GetCell(3);
                cell4.SetCellValue($"1.{i + 1}");
                cell4.CellStyle = AlignCenterCellStyle(workbook, sheet);

                var cell5 = sheet.GetRow(4 + i).GetCell(4);
                cell5.SetCellValue(key);
                cell5.CellStyle = AlignLeftCellStyle(workbook, sheet);

                var cell6 = sheet.GetRow(4 + i).GetCell(6);
                cell6.SetCellValue(dictionary[key]);
                cell6.CellStyle = HeaderCellStyle(workbook, sheet);

                i++;
            }
        }

        private static void CreateFootCell(ISheet sheet, IWorkbook workbook, Dictionary<string, double> dictionary)
        {
            var cell1 = sheet.GetRow(dictionary.Keys.Count + 4).GetCell(1);
            cell1.SetCellValue("小计：");
            cell1.CellStyle = HeaderCellStyle(workbook, sheet);

            sheet.AddMergedRegion(new CellRangeAddress(dictionary.Keys.Count + 5, dictionary.Keys.Count + 5, 1, 6));

            sheet.AddMergedRegion(new CellRangeAddress(dictionary.Keys.Count + 6, dictionary.Keys.Count + 6, 1, 5));
            var cell2 = sheet.GetRow(dictionary.Keys.Count + 6).GetCell(1);
            cell2.SetCellValue("合计：");
            cell2.CellStyle = HeaderCellStyle(workbook, sheet);

            var sum = dictionary.Values.Sum();
            var cell3 = sheet.GetRow(dictionary.Keys.Count + 4).GetCell(6);
            cell3.SetCellValue(sum);
            cell3.CellStyle = HeaderCellStyle(workbook, sheet);

            var cell4 = sheet.GetRow(dictionary.Keys.Count + 6).GetCell(6);
            cell4.SetCellValue(sum);
            cell4.CellStyle = HeaderCellStyle(workbook, sheet);
        }

        private static ICellStyle HeaderCellStyle(IWorkbook workbook, ISheet sheet)
        {
            var cellStyleFont = workbook.CreateFont(); //创建字体
            cellStyleFont.IsBold = true;
            cellStyleFont.FontHeightInPoints = 11; //字体大小
            cellStyleFont.FontName = "宋体"; //字体（仿宋，楷体，宋体 ）

            ICellStyle headerCellStyle = sheet.Workbook.CreateCellStyle();
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.BorderBottom = BorderStyle.Thin;
            headerCellStyle.BorderLeft = BorderStyle.Thin;
            headerCellStyle.BorderRight = BorderStyle.Thin;
            headerCellStyle.BorderTop = BorderStyle.Thin;
            headerCellStyle.SetFont(cellStyleFont);

            return headerCellStyle;
        }

        private static ICellStyle AlignCenterCellStyle(IWorkbook workbook, ISheet sheet)
        {
            ICellStyle alignCenterCellStyle = sheet.Workbook.CreateCellStyle();
            alignCenterCellStyle.Alignment = HorizontalAlignment.Center;
            alignCenterCellStyle.VerticalAlignment = VerticalAlignment.Center;
            alignCenterCellStyle.BorderBottom = BorderStyle.Thin;
            alignCenterCellStyle.BorderLeft = BorderStyle.Thin;
            alignCenterCellStyle.BorderRight = BorderStyle.Thin;
            alignCenterCellStyle.BorderTop = BorderStyle.Thin;

            return alignCenterCellStyle;
        }

        private static ICellStyle AlignLeftCellStyle(IWorkbook workbook, ISheet sheet)
        {
            var cellStyleFont = workbook.CreateFont(); //创建字体
            cellStyleFont.FontHeightInPoints = 10; //字体大小
            cellStyleFont.FontName = "宋体"; //字体（仿宋，楷体，宋体 ）

            ICellStyle alignCenterCellStyle = sheet.Workbook.CreateCellStyle();
            alignCenterCellStyle.BorderBottom = BorderStyle.Thin;
            alignCenterCellStyle.BorderLeft = BorderStyle.Thin;
            alignCenterCellStyle.BorderRight = BorderStyle.Thin;
            alignCenterCellStyle.BorderTop = BorderStyle.Thin;

            alignCenterCellStyle.SetFont(cellStyleFont);

            return alignCenterCellStyle;
        }

        private static void SaveFile(IWorkbook workbook)
        {
            var path = SaveToPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileFullName = Path.Combine(path, $"{DateTime.Now.AddMonths(-1):yyyy年MM月}开发项目明细_{UserName}.xlsx");

            // 将表格写入文件流
            var createStream = new FileStream(fileFullName, FileMode.Create, System.IO.FileAccess.Write);
            workbook.Write(createStream);
            createStream.Close();
        }
    }
}
