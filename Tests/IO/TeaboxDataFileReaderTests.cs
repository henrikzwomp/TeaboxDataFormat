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
                ParseLine(line, false, out comment, out type, out data);
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

        [Test]
        public void WillNotTrimAwayEmptyColumn()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("\tData2\tData3   ", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo(""));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));

            TestReader.PublicParseLine("\tData2\tData3\t", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo(""));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));

            TestReader.PublicParseLine("\t  Data2\tData3   ", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data[0], Is.EqualTo(""));
            Assert.That(data[1], Is.EqualTo("Data2"));
            Assert.That(data[2], Is.EqualTo("Data3"));
        }

        [Test]
        public void CanHandleStriktCommentRules()
        {
            /*
             * //Comment
             *  //CommentWithWhiteSpaceFirst
             * Data1\tDataWith//CommentIdentifier\tData2
             * Data//FaultyComment
             * Data //CorrectComment
             * Data\t//CorrectComment
             * http://TheReasonForThisStrickness
             */

            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("//Comment", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo("Comment"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine(" //CommentWithWhiteSpaceFirst", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo("CommentWithWhiteSpaceFirst"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(data.Length, Is.EqualTo(0));

            TestReader.PublicParseLine("Data1\tDataWith//CommentIdentifier\tData2", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data.Length, Is.EqualTo(3));
            Assert.That(data[0], Is.EqualTo("Data1"));
            Assert.That(data[1], Is.EqualTo("DataWith//CommentIdentifier"));
            Assert.That(data[2], Is.EqualTo("Data2"));

            TestReader.PublicParseLine("Data//FaultyComment", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("Data//FaultyComment"));

            TestReader.PublicParseLine("Data //CorrectComment", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo("CorrectComment"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("Data"));

            TestReader.PublicParseLine("Data\t//CorrectComment", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo("CorrectComment"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("Data"));

            TestReader.PublicParseLine("http://TheReasonForThisStrickness", out comment, out type, out data);

            Assert.That(comment, Is.EqualTo(""));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("http://TheReasonForThisStrickness"));
        }

        [Test]
        public void CanHandleUrlWithCommentOnEnd()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("https://www.avanza.se/aktier/om-aktien.html/98412/hms-networks // Stockholmsbörsen", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("https://www.avanza.se/aktier/om-aktien.html/98412/hms-networks"));
            Assert.That(comment, Is.EqualTo(" Stockholmsbörsen"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));
        }

        [Test]
        public void CanHandleMultipleCommentIdentifier()
        {
            string comment;
            TeaboxDataLineType type;
            string[] data;

            TestReader.PublicParseLine("Data //Comment //Another Comment", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(1));
            Assert.That(data[0], Is.EqualTo("Data"));
            Assert.That(comment, Is.EqualTo("Comment //Another Comment"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Data));

            TestReader.PublicParseLine("//Comment //Another Comment", out comment, out type, out data);

            Assert.That(data.Length, Is.EqualTo(0));
            Assert.That(comment, Is.EqualTo("Comment //Another Comment"));
            Assert.That(type, Is.EqualTo(TeaboxDataLineType.Other));
        }
    }
}
