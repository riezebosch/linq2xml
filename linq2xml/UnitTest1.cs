using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using System.IO;

namespace linq2xml
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WatIsLinq()
        {
            int[] items = { 0, 1, 1, 2, 3, 5, 8, 13, 20, 40 };
            var query = items.Where(i => i > 6);

            Console.WriteLine(string.Join(", ", query));
        }

        [TestMethod]
        public void WatIsDanDeQuerySyntaxOfComprehensionSyntax()
        {
            int[] items = { 0, 1, 1, 2, 3, 5, 8, 13, 20, 40 };
            var query = from i in items
                        where i > 6
                        select i;

            Console.WriteLine(string.Join(", ", query));
        }

        [TestMethod]
        public void WatIsDanLinqToXml()
        {
            var doc = XDocument.Load("books.xml");
            var query = doc
                .Element("catalog")
                .Elements("book")
                .Elements("author")
                .Select(e => e.Value);

            Console.WriteLine(string.Join(", ", query));
        }

        [TestMethod]
        public void HoeWerktLinq2XmlMetNamespaces()
        {
            var doc = XDocument.Load("books-ns.xml");
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";
            var query = doc
                .Element(tns + "catalog")
                .Elements(tns + "book")
                .Elements(tns + "author")
                .Select(e => e.Value);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void HoeWerktDeRecursieveXPathSelectorInLinq2Xml()
        {
            var doc = XDocument.Load("books-ns.xml");
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";

            var query = doc
                .Descendants(tns + "book")
                .Elements(tns + "author")
                .Select(e => e.Value);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void HoeOmgaanMetDefaultValues()
        {
            var doc = XDocument.Load("books-ns-empty-element.xml");
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";

            var query = doc
                .Descendants(tns + "book")
                .Select(book => (decimal?)book.Element(tns + "price") ?? 0);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void OpslaanVanEenAangepastDocument()
        {
            var doc = XDocument.Load("books-ns.xml");
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";
            var item = doc
                .Element(tns + "catalog")
                .Element(tns + "book")
                .Element(tns + "author");

            item.Value = "Christian Nagel";

            string file = "books-but-saved.xml";
            doc.Save(file);

            File.ReadAllText(file).ShouldContain("Christian Nagel");
        }

        [TestMethod]
        public void OpbouwenVanXmlVanuitCode()
        {
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";
            var doc = new XDocument(
                new XElement(tns + "catalog", 
                    new XElement(tns + "book", 
                        new XElement(tns + "author", "Christian Nagel"),
                        new XAttribute("id", "bk101")
                    )
                )
            );

            doc.Save("books-created-from-code.xml");
        }

        [TestMethod]
        public void OpbouwenVanXmlVanuitCodeMetXmlNamespacePrefix()
        {
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";
            XNamespace other = "urn:infosupport-com:xsd:authors";

            var doc = new XDocument(
                new XElement(tns + "catalog",
                    new XElement(tns + "book",
                        new XElement(other + "author", "Christian Nagel"),
                        new XAttribute("id", "bk101")
                    ),
                    new XAttribute(XNamespace.Xmlns + "a", "urn:infosupport-com:xsd:authors")
                )
            );

            doc.Save("books-created-from-code-met-ns.xml");
        }

        [TestMethod]
        public void ZoekenOpAttribute()
        {
            var doc = XDocument.Load("books-ns.xml");
            XNamespace tns = "urn:infosupport-com:xsd:linq2xml";
            var item = (string)doc
                .Element(tns + "catalog")
                .Elements(tns + "book")
                .Where(book => (string)book.Attribute("id") == "bk102")
                .First()
                .Element(tns + "author");

            item.ShouldBe("Ralls, Kim");
        }
    }
}
