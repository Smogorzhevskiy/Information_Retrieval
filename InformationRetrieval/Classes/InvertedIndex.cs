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

        public void CreateInvertedIndex( string stem)
        {
            invertedIndex = new SortedDictionary<string, Termin>();
            //Dictionary<int, Dictionary<string, string>> data = GetDataForIndex(stem);
            //foreach (int id in data.Keys)
            //{
                //foreach (string type in data[id].Keys)
                //{
                    //List<string> content = data[id][type].Trim().Split(' ').ToList();
                    AddToIndex(stem);
            //    }
            //}


        }

        //public void GetAnd(string term1, string term2){
        //    var resAnd = invertedIndex.And(term1, term2).ToList();
        //    File.WriteAllLines(@"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/andResult.txt", resAnd);

        //}

        //public void GetOr(string term1, string term2)
        //{
        //    var resOr = invertedIndex.Or(term1, term2).ToList();
        //    File.WriteAllLines(@"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/orResult.txt", resOr);

        //}

        public SortedDictionary<string, Termin> GetInvertedIndex(){
            return invertedIndex;
        }

        private void AddToIndex( string stem)
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
                            //Dictionary<int, Dictionary<string, int>> doc = new Dictionary<int,Dictionary<string, int>>();
                            //Dictionary<string, int> typeCount = new Dictionary<string, int>();
                            termin.AddId(id);
                            termin.AddType(id, type);

                            //doc.Add(id, new List<string> { type });
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

                //for (int i = 0; i < words.Count; i++)
                //{
                //    if (!invertedIndex.ContainsKey(words[i]))
                //    {
                //        Dictionary<string, List<int>> doc = new Dictionary<string, List<int>>();
                //        doc.Add(document, new List<int>{i});
                //        invertedIndex.Add(words[i], doc);
                //    }
                //    else
                //    {
                //        if (invertedIndex[words[i]].ContainsKey(document))
                //        {
                //            invertedIndex[words[i]][document].Add(i);
                //        }
                //        else{
                //            invertedIndex[words[i]].Add(document, new List<int> { i });
                //        }
                //    }
                //}
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
            Console.WriteLine("Count id = " + sp.Count());
            int j = 0;
            for (int i = 0; i < sp.Count; i+=2)
            {
                Dictionary<string, string> f = new Dictionary<string, string>();
                f.Add("title", RemoveFormulasHTML(sp[i]));
                f.Add("article", RemoveFormulasHTML(sp[i+1]));
                d.Add(j, f);
                j++;
                //r += sp[i];
            }

            //r = RemoveFormulasHTML(r);

            //File.WriteAllText(_file + "SupportFiles/for_" + stem + ".txt", r);

            //File.WriteAllLines(_file + "SupportFiles/for_" + stem + ".txt", sp);
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

    //TODO сделать для неограниченного числа терминов 
    //public static class Extensions
    //{ 
    //    public static IEnumerable<string> And(this Dictionary<string, Dictionary<string, List<int>>> index, string firstTerm, string secondTerm)
    //    {
    //        return (from d in index
    //                where d.Key.Equals(firstTerm)
    //                select d.Value.Keys.ToList()).SelectMany(x => x).Intersect
    //                        ((from d in index
    //                          where d.Key.Equals(secondTerm)
    //                          select d.Value.Keys.ToList()).SelectMany(x => x));
    //    }

    //    public static IEnumerable<string> Or(this Dictionary<string, Dictionary<string, List<int>>> index, string firstTerm, string secondTerm)
    //    {
    //        return (from d in index
    //                where d.Key.Equals(firstTerm) || d.Key.Equals(secondTerm)
    //                select d.Value.Keys.ToList()).SelectMany(x => x).Distinct();
    //    }
    //}

}

