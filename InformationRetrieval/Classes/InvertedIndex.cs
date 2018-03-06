using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace InformationRetrieval.Classes
{
    public class InvertedIndex
    {
         public Dictionary<string, Dictionary< string, List<int>>> invertedIndex;

        public void CreateInvertedIndex()
        {
            invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>();
            string folder = @"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/SupportFiles/";

            foreach (string file in Directory.EnumerateFiles(folder, "*.txt"))
            {
                List<string> content = File.ReadAllText(file).Trim().Split(' ').ToList();
                AddToIndex(content, file.Replace(folder, ""));
            }


        }

        public void GetAnd(string term1, string term2){
            var resAnd = invertedIndex.And(term1, term2).ToList();
            File.WriteAllLines(@"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/andResult.txt", resAnd);

        }

        public void GetOr(string term1, string term2)
        {
            var resOr = invertedIndex.Or(term1, term2).ToList();
            File.WriteAllLines(@"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/orResult.txt", resOr);

        }

        public Dictionary<string, Dictionary<string, List<int>>> GetInvertedIndex(){
            return invertedIndex;
        }

        private void AddToIndex(List<string> words, string document)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (!invertedIndex.ContainsKey(words[i]))
                {
                    Dictionary<string, List<int>> doc = new Dictionary<string, List<int>>();
                    doc.Add(document, new List<int>{i});
                    invertedIndex.Add(words[i], doc);
                }
                else
                {
                    if (invertedIndex[words[i]].ContainsKey(document))
                    {
                        invertedIndex[words[i]][document].Add(i);
                    }
                    else{
                        invertedIndex[words[i]].Add(document, new List<int> { i });
                    }
                }
            }
        }


    }

    //TODO сделать для неограниченного числа терминов 
    public static class Extensions
    { 
        public static IEnumerable<string> And(this Dictionary<string, Dictionary<string, List<int>>> index, string firstTerm, string secondTerm)
        {
            return (from d in index
                    where d.Key.Equals(firstTerm)
                    select d.Value.Keys.ToList()).SelectMany(x => x).Intersect
                            ((from d in index
                              where d.Key.Equals(secondTerm)
                              select d.Value.Keys.ToList()).SelectMany(x => x));
        }

        public static IEnumerable<string> Or(this Dictionary<string, Dictionary<string, List<int>>> index, string firstTerm, string secondTerm)
        {
            return (from d in index
                    where d.Key.Equals(firstTerm) || d.Key.Equals(secondTerm)
                    select d.Value.Keys.ToList()).SelectMany(x => x).Distinct();
        }
    }

}

