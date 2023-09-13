using AngleSharp.Html.Dom;
using System.Collections.Generic;
using ParserDoski.Core.Core;
using System.Net.Http;
using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Threading.Tasks;

namespace ParserDoski.Core._1000Dosok
{
    internal class DoskiParser : IParser<Task<string[]>>
    {

        private readonly HttpClient httpClient;
        
        public DoskiParser()
        {
            httpClient = new HttpClient();
            
        }

        public async Task<string[]> Parse(IHtmlDocument document, string linksPathSave)
        {
            var list = new List<string>();
            var items = document.QuerySelectorAll("p[style='margin-left:10;line-height:20px;'] a");

            foreach (var item in items)
            {
                list.Add(item.GetAttribute("href"));
            }

            string[] linkResult = list.ToArray();

            //LinkList(linkResult);
            await SaveToExcel(linkResult, linksPathSave);

            return linkResult;
        }
        
        public  async Task<string[]> LinkParser(IHtmlDocument document, string dataPathSave)
        {
            var list = new List<string>();

            var category = document.QuerySelector("span.span-style-height")?.TextContent?.Trim();
            list.Add(category);

            var region = document.QuerySelector("div.col-lg-6.col-12")?.TextContent?.Trim();
            list.Add(region);

            var titles = document.QuerySelector("h1.h1title.word-break")?.TextContent?.Trim();
            list.Add(titles);
            
            var price = document.QuerySelector("div.board-view-price.price-currency")?.TextContent?.Trim();
            list.Add(price);

            var name = document.QuerySelector(".board-view-user-right")?.TextContent?.Trim();
            list.Add(name);

            var id = document.QuerySelector("div.btn-custom.btn-color-green.schema-color-button.show-phone.mb5")?.GetAttribute("data-id");

            var postData = new Dictionary<string, string>
            {
                { "id_ad", id },
                { "action", "show_phone" }
            };

            var response = httpClient.PostAsync("https://1000dosok.ru/systems/ajax/ads.php", new FormUrlEncodedContent(postData)).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            list.Add(responseContent);
            // Извлечение нужных данных из responseContent и добавление их в list

            string result = string.Join("^", list);

            await SaveToExcel(new string[] { result }, dataPathSave);

            return new string[] { result };

        }

        public async Task SaveToExcel(string[] data, string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            bool fileExists = file.Exists;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet;

                // Если файл существует, получаем первый лист
                if (fileExists)
                {
                    worksheet = package.Workbook.Worksheets[0];
                }
                else // Если файл не существует, создаем новый лист
                {
                    worksheet = package.Workbook.Worksheets.Add("Sheet1");
                }

                // Определяем, на какой строке начинается добавление новых данных
                int startRow = fileExists && worksheet.Dimension != null ? worksheet.Dimension.Rows + 1 : 1;


                // Добавляем новые данные в ячейки
                for (int i = 0; i < data.Length; i++)
                {
                    worksheet.Cells[startRow + i, 1].Value = data[i];
                }

                // Сохраняем изменения в файле
                package.Save();

                await Task.Delay(500);

                // Закрываем пакет Excel
                package.Dispose();
            }
        }



        /*public void WriteToCsv(string[] data, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (string line in data)
                {
                    string escapedLine = EscapeCsvValue(line);
                    writer.WriteLine(escapedLine);
                }
            }
        }

        private string EscapeCsvValue(string value)
        {
            // Если значение содержит запятые, кавычки или переносы строк,
            // обрамляем его в двойные кавычки и экранируем специальные символы
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }*/

    }
}
