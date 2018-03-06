using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace InformationRetrieval
{
    public class DocumentData
    {
        public string _url = null;
        public HtmlDocument _doc = new HtmlDocument();
        private string _file = @"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/";


        public DocumentData(string url)
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
        public void GetHtmlDoc(string url, HtmlDocument doc)
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
                File.WriteAllText(_file + "FilesXML/XML_For_HomeWork2.1.xml", saveDoc.ToString());

                Console.WriteLine("Succsess");


            }

        }
        public void GetTextFromXML(string stem){
            XDocument doc = XDocument.Load(_file + "FilesXML/XML_For_HomeWork2.1.xml");
            var sp = doc.Document.Descendants()
                        .Elements(stem).Select(x => x.Value).ToList();
            string r = null;
            for (int i = 0; i < sp.Count; i++)
            {
                r += sp[i];
            }
                
            r = RemoveFormulasHTML(r);

            File.WriteAllText(_file + "SupportFiles/for_" + stem + ".txt", r);

            //File.WriteAllLines(_file + "SupportFiles/for_" + stem + ".txt", sp);
            Console.WriteLine("Succsess of creating a doc for " + stem);
        


        }

        //удаление формул 
        private string RemoveFormulasHTML(string text){
            string rec = null;
            Regex regex = new Regex(@"([$].*[$])|([$].*[\\].*)|(^[\\].*)|([a-z, 0-10].*[$])|([$][a-z].*)|([(,)])|([a-z , A-Z]*[?])|([?])");
            string[] str;
            str = text.Split(' ');
            for (int i = 0; i < str.Count(); i++){
                if (regex.Replace(str[i], " ").Trim() != "")
                rec += regex.Replace(str[i], " ").Trim() + " ";

            }
            return rec;
        }

        //создание xml для инвертированного индекса 
        public void XMLForInvertedIndex( Dictionary<string, Dictionary<string, List<int>>> invertedIndex){
            XElement documents = new XElement("documents");
            XElement doc1 = new XElement("doc", new XAttribute("id", 1));
            XElement doc2 = new XElement("doc", new XAttribute("id", 2));

            foreach(string keyTerm in invertedIndex.Keys){
                Dictionary<string, List<int>> docs = invertedIndex[keyTerm];
                foreach(string docTerm in docs.Keys){
                    
                        int count = docs[docTerm].Count();
                        XAttribute quantity = new XAttribute("quantity", count);
                        XAttribute name = new XAttribute("name", keyTerm);
                        XElement location = new XElement("location");
                        for (int i = 0; i < count; i++){
                            XElement loc = new XElement("loc", docs[docTerm][i]);
                            location.Add(loc);
                        }
                        XElement term = new XElement("termin", name, quantity, location);

                    if (docTerm == "for_porter.txt")
                        doc1.Add(term);
                    else
                        doc2.Add(term);

                }
            }

            documents.Add(doc1, doc2);

            XDocument saveDoc = new XDocument(documents);
            File.WriteAllText(_file + "FilesXML/XML_For_HomeWork3.xml", saveDoc.ToString());

            Console.WriteLine("Succsess create InvertedIndex");

        }

    }

}
