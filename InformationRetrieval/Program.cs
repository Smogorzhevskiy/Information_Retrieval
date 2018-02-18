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
    class MainClass
    {
        private static string _url = null;


        private static void Main(string[] args)
        {

            Console.WriteLine("Enter url");
            _url = Console.ReadLine();

            DocumentData document= new DocumentData(_url);
            document.CreateXMLDoc();

        }

    }
}
