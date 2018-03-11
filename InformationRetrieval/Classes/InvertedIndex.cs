using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Iveonik.Stemmers;

namespace InformationRetrieval.Classes
{
    public class InvertedIndex
    {
        string _stem;
        string folder = @"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/";

        public SortedDictionary<string, Termin> invertedIndex;

        public void CreateInvertedIndex(string stem)
        {
            invertedIndex = new SortedDictionary<string, Termin>();
            AddToIndex(stem);
            _stem = stem;
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

        // булевый поиск с и
        public void And(string input){
            string output = null; 
            string[] s = input.Split(' ');
            if (Check(s))
            {
                Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
                dic = SplitItems(s);
                List<int> a = new List<int>();
                if (dic.ContainsKey(1))
                {
                    a = Inter(dic[1]);
                }
                else
                {
                    foreach (string key in invertedIndex.Keys){
                        foreach(int i in invertedIndex[key].Keys()){
                            if (!a.Contains(i)){
                                a.Add(i);
                            }

                        }

                    }

                }
                List<int> n = new List<int>();

                if (dic.ContainsKey(0))
                {
                    n = DeletNot(a, GetIDs(dic[0]));
                }
                else
                {
                    n = a;
                }

                for (int i = 0; i < n.Count(); i++)
                {
                    output += n[i] + " ";
                }
                if (output ==null)
                {
                    File.WriteAllText(folder + "FilesXML/HomeWork4.txt", input + " :");
                }
                else
                {
                    File.WriteAllText(folder + "FilesXML/HomeWork4.txt", input + " : " + output);
                }


            }
            else 
            {
                File.WriteAllText(folder + "FilesXML/HomeWork4.txt",input + " : ");

            }
        }
        // проверка наоичия терминов, кроме терминов с -, иначе не будут найдены сочетания терминов которые и так не содержали не сущ термин
        public bool Check(string [] s){
            List<string> output = new List<string>(s.Count());
            Regex regex = new Regex(@"(^[-])");
            Console.WriteLine("Check " + s.Count());

            for (int i = 0; i < s.Count(); i++)
            {
                if (!regex.IsMatch(s[i]) && !invertedIndex.ContainsKey(Convert(s[i])))
                {
                    return false;
                }
                Console.WriteLine("Check " + Convert(s[i]));
               
        }
            return true;

        }

        // получить документы 
        public Dictionary<int, List<int>> GetIDs(List<string> terms){
            Dictionary<int, List<int>> d = new Dictionary<int, List<int>>();
            for (int i = 0; i < terms.Count(); i++)
            {

                if (invertedIndex.ContainsKey(terms[i]))
                {
                    d.Add(i, invertedIndex[terms[i]].Keys());
                    Console.WriteLine("a = " + Convert(terms[i]));

                }
            }

            return d;
        }

        //поиск содержания выражения в документах
        public List<int> Inter(List<string> input)
        {
            Dictionary<int, List<int>> d = GetIDs(input);
            List<int> l = new List<int>();
            l = d[0];
            for (int i = 1; i < d.Count(); i++)
            {
                l = Intersect(l, d[i]);
            }
            for (int i = 0; i < l.Count(); i++)
            {
                Console.WriteLine("l = " + l[i]);

            }
          
            return l;
        }


        public List<int> Intersect(List<int> a, List<int> b){
            List<int> output = new List<int>();
            for (int i = 0; i < a.Count(); i++){
                for (int j = 0; j < b.Count(); j++){
                    if (a[i] == b[j])
                        output.Add(a[i]);
                }
            }

            return output;
        }

        // удаление документов с -
        public List<int> DeletNot(List<int> a, Dictionary<int, List<int>> b)
        {
           
            List<int> output = new List<int>();
            output = a;
            for (int i = 0; i < a.Count(); i++)

            {
                for (int j = 0; j < b.Count(); j++)

                {
                    for (int g = 0; g < b[j].Count(); g++)
                    {
                        if (a.Contains(b[j][g]))
                        {
                            output.Remove(b[j][g]);
                        }
                    }
                }
            }


            return output;
       
        }

        // разделить термины на термины с - и без 
        public Dictionary<int, List<string>> SplitItems(string [] list)
        {
            Dictionary<int, List<string>> output = new Dictionary<int, List<string>>();
            Regex regex = new Regex(@"(^[-])");
            for (int i = 0; i < list.Count(); i++)
            {
                
                if (regex.IsMatch(list[i]))
                {

                    
                    if (!output.ContainsKey(0)){
                        output.Add(0, new List<string>());
                    }
                    string n = regex.Replace(list[i], "");
                    output[0].Add(Convert(n));
                }
                else
                {
                    if (!output.ContainsKey(1))
                    {
                        output.Add(1, new List<string>());
                    }
                    output[1].Add(Convert(list[i]));
                }
            }


            return output;
        }

        // конвертировать в зависимости от стеммера
        public string Convert(string input){

           string output = null;
            if (_stem == "porter"){
                RussianStemmer stemmer = new RussianStemmer();
                    output = stemmer.Stem(input);
                return output;
            }
            if (_stem == "mystem")
            {
                MyStem stemmer = new MyStem();
                output = stemmer.Stemer(input);
                return output;
            }
            return null;

        }
    }

}

