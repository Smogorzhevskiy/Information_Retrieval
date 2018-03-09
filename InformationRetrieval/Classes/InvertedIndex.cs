using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace InformationRetrieval.Classes
{
    public class InvertedIndex
    {
        string folder = @"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/";

        public SortedDictionary<string, Termin> invertedIndex;

        public void CreateInvertedIndex(string stem)
        {
            invertedIndex = new SortedDictionary<string, Termin>();
            AddToIndex(stem);
        }

        public SortedDictionary<string, Termin> GetInvertedIndex()
        {
            return invertedIndex;
        }

        private void AddToIndex(string stem)
        {
            Dictionary<int, Dictionary<string, string>> data = GetDataForIndex(stem);
            foreach (int id in data.Keys)
            {
                foreach (string type in data[id].Keys)
                {
                    List<string> content = data[id][type].Trim().Split(' ').ToList();

                    for (int i = 0; i < content.Count; i++)
                    {
                        if (!invertedIndex.ContainsKey(content[i]))
                        {
                            Termin termin = new Termin(content[i]);
                            termin.AddId(id);
                            termin.AddType(id, type);
                            invertedIndex.Add(content[i], termin);
                        }
                        else
                        {
                            invertedIndex[content[i]]._count++;
                            if (invertedIndex[content[i]].CContainsKey(id))
                            {
                                invertedIndex[content[i]].AddType(id, type);
                            }
                            else
                            {
                                invertedIndex[content[i]].AddId(id);
                                invertedIndex[content[i]].AddType(id, type);
                            }
                        }
                    }
                }
            }
        }


        // получаю словарь из XML с id, артиклем и названием статьи 
        public Dictionary<int, Dictionary<string, string>> GetDataForIndex(string stem)
        {
            XDocument doc = XDocument.Load(folder + "FilesXML/XML_For_HomeWork2.1.xml");
            var sp = doc.Document.Descendants()
                        .Elements(stem).Select(x => x.Value).ToList();
            //string r = null;
            Dictionary<int, Dictionary<string, string>> d = new Dictionary<int, Dictionary<string, string>>();
            int j = 0;
            for (int i = 0; i < sp.Count; i += 2)
            {
                Dictionary<string, string> f = new Dictionary<string, string>();
                f.Add("title", RemoveFormulasHTML(sp[i]));
                f.Add("article", RemoveFormulasHTML(sp[i + 1]));
                d.Add(j, f);
                j++;
            }
            Console.WriteLine("Succsess of creating a doc for " + stem);
            return d;
        }

        //удаление формул 
        private string RemoveFormulasHTML(string text)
        {
            string rec = null;
            Regex regex = new Regex(@"([$].*[$])|([$].*[\\].*)|(^[\\].*)|([a-z, 0-10].*[$])|([$][a-z].*)|([(,)])|([a-z , A-Z]*[?])|([?])");
            string[] str;
            str = text.Split(' ');
            for (int i = 0; i < str.Count(); i++)
            {
                if (regex.Replace(str[i], " ").Trim() != "")
                    rec += regex.Replace(str[i], " ").Trim() + " ";
            }
            return rec;
        }
    }
}

