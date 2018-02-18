using System.Xml.Linq;
using HtmlAgilityPack;
namespace InformationRetrieval
{
    public class Issue
    {
        XAttribute _href;
        XElement _articles = new XElement("articles");
        public Issue(string url)
        {
            _href = new XAttribute("url", url);

        }

        public void AddArticle(XElement articl)
        {
            _articles.Add(articl);
        }

        public XElement GetIssue()
        {
            XElement _issue = new XElement("issue", _href);
            _issue.Add(_articles);
            return _issue;
        }


    }
}
