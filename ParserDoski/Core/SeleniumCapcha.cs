using AngleSharp.Html.Dom;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace ParserDoski.Core
{
    internal class SeleniumCapcha
    {
        readonly HttpClient client;
        public bool isActive = false;

        /*public async Task CapchaInput(string capcha)
        {
            isActive = true;

            IWebDriver driver = new ChromeDriver();

            // Открытие страницы авторизации
            driver.Navigate().GoToUrl("https://1000dosok.ru");

            // Ожидание появления элемента на странице
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            await Task.Delay(500);

            // Ввод логина и пароля
            var capchaElement =  driver.FindElement(By.XPath("//input[@name='vvod']"));
            capchaElement.SendKeys(capcha);

            await Task.Delay(200);

            var submitButton = driver.FindElement(By.XPath("//input[@name='B1']"));
            submitButton.Click();

            await Task.Delay(1000);

            driver.Quit();
            driver.Dispose();

            isActive = false;
        }*/

        /*public async Task CapchaSekenium(IHtmlDocument document)
        {
            var value = document.QuerySelector("input[name=B1]").GetAttribute("value");
            var bElements = document.QuerySelectorAll("b");
            foreach (var bElement in bElements)
            {
                string code = bElement.TextContent;
                bool isValidCode = code.All(char.IsDigit);
                if (isValidCode)
                {
                    if (int.TryParse(code, out int number))
                    {
                        await CapchaInput(number.ToString());
                    }
                }
            }
        }*/


        public async Task Capcha(IHtmlDocument document)
        {
            var value = document.QuerySelector("input[name=B1]").GetAttribute("value");
            var bElements = document.QuerySelectorAll("b");
            foreach (var bElement in bElements)
            {
                string code = bElement.TextContent;
                bool isValidCode = code.All(char.IsDigit);
                if (isValidCode)
                {
                    if (int.TryParse(code, out int number))
                    {
                        var url = "https://1000dosok.ru/images/proverka.php";
                        var parameters = new Dictionary<string, string>
                        {
                            { "vvod", number.ToString() },
                            { "vvod2", "http://1000dosok.ru/index.php" }
                        };

                        using (var httpClientHandler = new HttpClientHandler())
                        {
                            var cookieContainer = new CookieContainer();
                            cookieContainer.Add(new Uri(url), new Cookie("PHPSESSID", "929319fb37d71ef5824014039e675d68"));

                            httpClientHandler.CookieContainer = cookieContainer;

                            using (var httpClient = new HttpRequestMessage(HttpMethod.Post, url))
                            {
                                // Установка заголовков
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("DNT", "1");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Length", "93");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://1000dosok.ru/");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://1000dosok.ru");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Not.A / Brand\";v=\"8\", \"Chromium\";v=\"114\", \"Google Chrome\";v=\"114\"");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "1000dosok.ru");

                                var content = new FormUrlEncodedContent(parameters);

                                var response = await client.PostAsync(url, content);
                                var responseContent = await response.Content.ReadAsStringAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}
