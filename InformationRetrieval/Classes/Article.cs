using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using Iveonik.Stemmers;

namespace InformationRetrieval
{
    public class Article
    {

        string _url;
        string _title;
        string _annotation;
        string _annotationPorter;
        string _annotationMyStem;
        string _keywords;

        public Article(HtmlNode node, string url, HtmlDocument doc)
        {
            _url = url;
            _title = node.InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-");
            _annotation = Annotation(doc);
            _annotationPorter = AnnotationForPorter(_annotation);
            _annotationMyStem = AnnotationForMyStem(_annotation);
            _keywords = Keywords(doc);
        }


        private string Annotation(HtmlDocument doc)
        {
            string annotation = null;
            HtmlNodeCollection annotations = doc.DocumentNode.SelectNodes("//b[contains(text(),'Аннотация')]/following::text()[preceding::b[1][contains(text(),'Аннотация')] and not(parent::b)]");
            for (int i = 0; i < annotations.Count; i++)
            {
                // пропуск нормированных пробелов и тире
                annotation += annotations[i].InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-").Replace("&", "");
            }

            return annotation.Trim();
        }

        private string AnnotationForPorter(string annotation)
        {


            RussianStemmer stemmer = new RussianStemmer();

            string annotationPorter = null;
            foreach (string word in annotation.Split(' ', ',', '.'))
            {
                annotationPorter += stemmer.Stem(word) + " ";
            }
            return annotationPorter;

        }

        private string AnnotationForMyStem(string annotation)
        {

            string stem = MyStem(annotation);
            return stem;
        }

        public static string MyStem(string _original)
        {
            string directory = "/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/SupportFiles";
            string result = "";

            File.WriteAllText(directory + "/input.txt", _original, Encoding.GetEncoding("Utf-8"));
            Console.WriteLine(_original);
            Process process = new Process();
            process.StartInfo.FileName = "/Applications/Utilities/Terminal.app";
            // исполняющий скрипт для вызова mystem
            process.StartInfo.Arguments = directory + "/startStem.command";

            //TODO новые окна все равно создаются
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            process.Close();
            //приходится стопить программу, иначе запрос не успевает отработать и в результате значение нынешнего нода присвоится следующему и так по цепочке
            System.Threading.Thread.Sleep(1000);
            string[] lines = File.ReadAllLines(directory + "/output.txt");

            for (int i = 0; i < lines.Length; i++)
            {
                result += lines[i].Replace("{", "").Replace("}", " ");

            }

            return result;
        }

        private string Keywords(HtmlDocument doc)
        {
            string keywords = doc.DocumentNode.SelectSingleNode("//b[contains(.,'Ключевые')]/following-sibling::i").InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-").Replace("&", "");
            return keywords;

        }

        private XElement GetUrl()
        {
            return new XElement("url", _url);
        }

        private XElement GetTitle()
        {
            return new XElement("title", _title);
        }

        private XElement GetAnnotation()
        {
            XAttribute origin = new XAttribute("origin", _annotation);
            XAttribute porter = new XAttribute("porter", _annotationPorter);
            XAttribute mystem = new XAttribute("mystem", _annotationMyStem);

            return new XElement("annotation", origin, porter, mystem);
        }

        private XElement GetKeywords()
        {
            return new XElement("keywords", _keywords);
        }

        public XElement GetArticle()
        {
            XElement article = new XElement("article");
            article.Add(GetUrl(), GetTitle(), GetAnnotation(), GetKeywords());
            return article;
        }
    }

}
