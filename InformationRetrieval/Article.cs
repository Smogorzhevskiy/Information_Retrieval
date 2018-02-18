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
    public class Article
    {

        string _url;
        string _title;
        string _annotation;
        string _keywords;

        public Article(HtmlNode n, string url, HtmlDocument d)
        {
            _url = url;
            _title = n.InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-");
            _annotation = Annotation(d);
            _keywords = Keywords(d);
        }


        public static string Annotation(HtmlDocument doc)
        {
                string annotation = null;
            // чтобы получить аннотацию, приходится брать весь текст до ключевых слов, т.к. между текстом попадаются ноды с формулами  
                HtmlNodeCollection annotations = doc.DocumentNode.SelectNodes("//table//text()[preceding-sibling::b[contains(text(), 'Аннотация')] and following-sibling::b[contains(text(), 'Ключевые')]]");
                for (int i = 0; i < annotations.Count; i++)
                {
                // пропуск нормированных пробелов и тире
                    annotation += annotations[i].InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-");
                }

                return annotation;
        }

        public static string Keywords(HtmlDocument doc)
        {
            string keywords = doc.DocumentNode.SelectSingleNode("//b[contains(.,'Ключевые')]/following-sibling::i").InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-");
            return keywords;

        }

        public XElement GetUrl(){
            return new XElement("url", _url);
        }

        public XElement GetTitle() { 
            return new XElement("title", _title);
        }

        public XElement GetAnnotation() {
            return new XElement("annotation", _annotation);
        }

        public XElement GetKeywords() {
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
