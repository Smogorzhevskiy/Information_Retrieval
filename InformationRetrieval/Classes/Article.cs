using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using InformationRetrieval.Classes;
using Iveonik.Stemmers;

namespace InformationRetrieval
{
    public class Article
    {

        string _url;
        string _title;
        string _titlePorter;
        string _titleMyStem;
        string _annotation;
        string _annotationPorter;
        string _annotationMyStem;
        string[] _keywords;

        public Article(HtmlNode node, string url, HtmlDocument doc)
        {
            _url = url;
            _title = node.InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-");
            _titlePorter = TitleForPorter(_title);
            _titleMyStem = TitleForMyStem(_title);
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
                annotation += annotations[i].InnerText.Trim().Replace("&nbsp;", "").Replace("ndash;", "").Replace("&", "").Replace("[ ]+", " ");
            }

            return annotation.Trim();
        }

        private string AnnotationForPorter(string annotation)
        {


            RussianStemmer stemmer = new RussianStemmer();

            string annotationPorter = null;
            foreach (string word in annotation.Split(' ', ',', '.'))
            {
                if (word != "")
                {
                    annotationPorter += stemmer.Stem(word) + " ";
                }
            }
            return annotationPorter;

        }

        private string AnnotationForMyStem(string annotation)
        {

            MyStem stemmer = new MyStem();
            string stem = stemmer.Stemer(annotation);
            return stem;
        }

        private string TitleForPorter(string title)
        {


            RussianStemmer stemmer = new RussianStemmer();

            string titlePorter = null;
            foreach (string word in title.Split(' ', ',', '.'))
            {
                titlePorter += stemmer.Stem(word) + " ";
            }
            return titlePorter;

        }

        private string TitleForMyStem(string title)
        {
            MyStem stemmer = new MyStem();
            string stem = stemmer.Stemer(title);
            return stem;
        }


        private string[] Keywords(HtmlDocument doc)
        {
            string keywords = doc.DocumentNode.SelectSingleNode("//b[contains(.,'Ключевые')]/following-sibling::i").InnerText.Replace("&nbsp;", " ").Replace("ndash;", "-").Replace("&", "");
            string[] words = keywords.Split(',');
            return words;

        }

        private XElement GetUrl()
        {
            return new XElement("url", _url);
        }

        private XElement GetTitle()
        {
            XElement title = new XElement("title");
            title.Add(new XElement("origin", _title), new XElement("porter", _titlePorter), new XElement("mystem", _titleMyStem));
            return title;
        }

        private XElement GetAnnotation()
        {
            XElement annotation = new XElement("annotation");
            annotation.Add(new XElement("origin", _annotation), new XElement("porter", _annotationPorter), new XElement("mystem", _annotationMyStem));
            return annotation;
        }

        private XElement GetKeywords()
        {
            XElement keywords = new XElement("keywords");
            for (int i = 0; i < _keywords.Length; i++)
            {
                keywords.Add(new XElement("keyword", _keywords[i].Replace(".", "")));
            }
            return keywords;
        }

        public XElement GetArticle()
        {
            XElement article = new XElement("article");
            article.Add(GetUrl(), GetTitle(), GetAnnotation(), GetKeywords());
            return article;
        }
    }

}
