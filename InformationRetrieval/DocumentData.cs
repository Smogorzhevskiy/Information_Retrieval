using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace InformationRetrieval
{
    public class DocumentData
    {
        public string _url = null;
        public HtmlDocument _doc = new HtmlDocument();


        public DocumentData( string url)
        {
            _url = url;
            
            GetHtmlDoc(url, _doc);


        }


        // реквест запрос на страницу
        public string GetRequest(string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                Console.WriteLine(url);
                //httpWebRequest.AllowAutoRedirect = false;//Запрещаем автоматический редирект
                httpWebRequest.Method = "GET"; //Можно не указывать, по умолчанию используется GET.
                httpWebRequest.Referer = "http://google.com"; // Реферер. Тут можно указать любой URL
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var stream = httpWebResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding("Windows-1251")))

                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        // получение документа с Html кодом 
        public  void GetHtmlDoc(string url, HtmlDocument doc)
        {
            string content = GetRequest(url);
            doc.LoadHtml(content);
        }


        // получение коллекции нодов содержащих ссылки
        public HtmlNodeCollection GetUrlCollection(HtmlDocument doc)
        {
            // добавил colspan=2 и width=90% в условие, чтобы избежать попадания в коллекцию нодов с <a class="SLink" ...> ссылки в которых не ведут на статьи
            return doc.DocumentNode.SelectNodes("//td[contains(@colspan, '2') and contains(@width, '90%')]/a[contains(@class,'SLink')]");
        }

        // создание XML документа 
        public void CreateXMLDoc()
        {
            HtmlNodeCollection _nodeCollection = GetUrlCollection(_doc);
            if (_nodeCollection != null)
            {
                Issue issue = new Issue(_url);
                HtmlDocument newDoc = new HtmlDocument();

                // базовая часть ссылки
                var baseUrl = new Uri(_url);


                for (int i = 0; i < _nodeCollection.Count; i++)
                {
                    // иногда возвращается пустая страница, возможно не успевает обрабатывать
                    System.Threading.Thread.Sleep(1000);

                    // новая ссылка
                    var newUrl = new Uri(baseUrl, _nodeCollection[i].Attributes["href"].Value).AbsoluteUri;

                    //получаем Html код по новой ссылке 
                    GetHtmlDoc(newUrl, newDoc);

                    // создание и заполнение артикля нодами 
                    Article article = new Article(_nodeCollection[i], newUrl, newDoc);
                    XElement articleElement = article.GetArticle();
                    issue.AddArticle(articleElement);


                }

                // добавление issue и сохранение документа
                XDocument saveDoc = new XDocument(issue.GetIssue());
                File.WriteAllText(@"/Users/User/Documents/ITIS /Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/TestXML.xml", saveDoc.ToString());

                Console.WriteLine("Succsess");


            }

        }

    }

}
