﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace InformationRetrieval.Classes
{
    public class Termin
    {
        public string _value;
        public int _count = 1;
        public Dictionary<int, double> score = new Dictionary<int, double>();
        public  SortedDictionary<int, Dictionary<string, int>> _doc = new SortedDictionary<int, Dictionary<string, int>>();

        public Termin(string value)
        {
            _value = value;
            Dictionary<string, int> listType = new Dictionary<string, int>();

        }

        public int GetCount(){
            return _count;
        }

        public void AddType(int id, string type)
        {
            if (CContainsKey(id))
            {
                if (_doc[id].ContainsKey(type))
                {
                    _doc[id][type] = _doc[id][type]+1;
                }
                else
                {
                    _doc[id].Add(type, 1);
                }
            }
           
        }

        public void AddId(int id){
            Dictionary<string, int> typeDictionary = new Dictionary<string, int>();
            _doc.Add(id, typeDictionary);
        }

        public bool CContainsKey(int id){
            if (_doc.ContainsKey(id))
                return true;
            else
                return false;
        }

        public XElement TerminXML(){
            XAttribute value = new XAttribute("value", _value);
            XAttribute count = new XAttribute("count", _count);
            XElement termin = new XElement("termin", value, count);
            foreach (int id in _doc.Keys){
                XElement doc = new XElement("doc", new XAttribute("id", id), new XAttribute("score", score[id]));
                foreach (string type in _doc[id].Keys)
                {
                    int countWords = _doc[id][type];
                    XAttribute attribut = new XAttribute(type, countWords);
                    doc.Add(attribut);

                }
                termin.Add(doc);
            }
            return termin;
        }

        public List<int>  Keys(){
            return _doc.Keys.ToList();
        }
        public List<string> KeysForType(int id)
        {
            return _doc[id].Keys.ToList();
        }
    }
}
