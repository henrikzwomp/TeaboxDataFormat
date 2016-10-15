using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeaboxDataFormat.IO;

namespace TeaboxDataFormat.Tests.IO
{
    [TestFixture]
    public class TeaboxDataFileReaderTests
    {
        public class TestReader : TeaboxDataFileReader
        {
            public static void PublicParseLine(string line, out string comment
                , out TeaboxDataLineType type, out string[] data)
            {
                ParseLine(line, out comment, out type, out data);
            }
        }

        [Test]
        public void CanParseEmptyLines()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine("    ", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine("\t\t\t", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));
        }

        [Test]
        public void CanParseCommentedLines()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("// Hello", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(" Hello"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine("   // World", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(" World"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine("\t\t// Tab", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(" Tab"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));
        }

        [Test]
        public void CanParseTitles()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("!Title1\tTitle2\tTitle3", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Titles));
            Assert.That(data[0], Is.EqualTo("Title1"));
            Assert.That(data[1], Is.EqualTo("Title2"));
            Assert.That(data[2], Is.EqualTo("Title3"));
            Assert.That(comment, Is.EqualTo(""));

            TestReader.PublicParseLine("!Title1\tTitle2\tTitle3 // World", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Titles));
            Assert.That(data[0], Is.EqualTo("Title1"));
            Assert.That(data[1], Is.EqualTo("Title2"));
            Assert.That(data[2], Is.EqualTo("Title3"));
            Assert.That(comment, Is.EqualTo(" World"));
        }

        [Test]
        public void CanParseData()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("Data1\tData2\tData3", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo("Data1"));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));

            TestReader.PublicParseLine("Data1\tData2\tData3 // World", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo("Data1"));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));
            Assert.That(comment, Is.EqualTo(" World"));
        }

        [Test]
        public void WillTrimData()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine(" Data1 \t Data2 \t Data3", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo("Data1"));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));

            TestReader.PublicParseLine("Data1 \tData2 \t Data3 // World", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo("Data1"));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));
            Assert.That(comment, Is.EqualTo(" World"));
        }
    }
}
