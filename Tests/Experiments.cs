using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using TeaboxDataFormat.IO;

/// <summary>
/// Space for developing new features
/// </summary>

namespace Tests
{
    /*
        ToDo Fix so code can handled 3 annoying characters in the beginning of text file.
        https://en.wikipedia.org/wiki/Byte_order_mark
        http://stackoverflow.com/questions/2223882/whats-different-between-utf-8-and-utf-8-without-bom
    */

    [TestFixture]
    public class Experiments
    {
        [Test]
        public void GetDataAsWorksWithStringProperties()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>()
                {
                    "!DateField1\tDateField2",
                    "Hello\tWorld",
                    "// A bit of a comment", 
                    "Something\tElse",
                });

            var data_file = TeaboxDataFile.Open(file.Object);
            var result = data_file.GetDataAs<TestItemForGetDataAsWorksWithStringProperties>();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(x => x.DateField1 == "Hello" && x.DateField2 == "World"), Is.EqualTo(1));
            Assert.That(result.Count(x => x.DateField1 == "Something" && x.DateField2 == "Else"), Is.EqualTo(1));
        }

        public class TestItemForGetDataAsWorksWithStringProperties : TeaboxDataLine
        {
            [TeaboxData]
            public string DateField1 { get; set; }
            [TeaboxData]
            public string DateField2 { get; set; }
        }

        [Test]
        public void GetDataAsWorksWithIntAndDateTimeProperties()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>()
                {
                    "!MyRowID\tMyRowData\tMyEditDate",
                    "11\tHello\t2017-02-11",
                    "// A bit of a comment",
                    "22\tWorld\t2017-01-12",
                });

            var data_file = TeaboxDataFile.Open(file.Object);
            var result = data_file.GetDataAs<TestItemForGetDataAsWorksWithIntAndDateTimeProperties>();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(x => x.MyRowID == 11 && x.MyEditDate == new DateTime(2017, 2, 11)), Is.EqualTo(1));
            Assert.That(result.Count(x => x.MyRowID == 22 && x.MyEditDate == new DateTime(2017, 1, 12)), Is.EqualTo(1));
        }

        public class TestItemForGetDataAsWorksWithIntAndDateTimeProperties : TeaboxDataLine
        {
            [TeaboxData]
            public int MyRowID { get; set; }
            [TeaboxData]
            public DateTime MyEditDate { get; set; }
        }

        [Test]
        public void GetDataAsWorksWithBoolProperties()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>()
                {
                    "!Id\tWorks",
                    "1\tfalse",
                    "2\t",
                    "3\ttrue",
                });

            var data_file = TeaboxDataFile.Open(file.Object);
            var result = data_file.GetDataAs<TestItemForGetDataAsWorksWithBoolProperties>();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Count(x => x.Id == 1 && x.Works == false), Is.EqualTo(1));
            Assert.That(result.Count(x => x.Id == 2 && x.Works == false), Is.EqualTo(1));
            Assert.That(result.Count(x => x.Id == 3 && x.Works == true), Is.EqualTo(1));
        }

        public class TestItemForGetDataAsWorksWithBoolProperties : TeaboxDataLine
        {
            [TeaboxData]
            public int Id { get; set; }
            [TeaboxData]
            public bool Works { get; set; }
        }

        [Test]
        public void CanMergeData()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>()
                {
                    "!DateField1\tDateField2",
                    "Hello\tWorld",
                    "// A bit of a comment",
                    "Something\tElse",
                });

            IList<string> result = new List<string>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => { result = y; });

            var data_file = TeaboxDataFile.Open(file.Object);
            var data = data_file.GetDataAs<TestItemForGetDataAsWorksWithStringProperties>();

            data.First(x => x.DateField1 == "Hello").DateField2 = "Me";
            data.First(x => x.DateField1 == "Something").DateField2 = "Wicked";

            data_file.MergeData<TestItemForGetDataAsWorksWithStringProperties>(data, "DateField1");
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("!DateField1\tDateField2"));
            Assert.That(result[1], Is.EqualTo("Hello\tMe"));
            Assert.That(result[2], Is.EqualTo("// A bit of a comment"));
            Assert.That(result[3], Is.EqualTo("Something\tWicked"));
        }

        /*// TickerSymbol\tLastDownload\tFailed
!TickerSymbol\tLastDownload\tFailed
EDGE.ST\t2017-02-18 09:00:00\t
SHOT.ST\t2017-02-18 09:00:00\t*/

        [Test]
        public void xxx2()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>()
                {
                    "// TickerSymbol\tLastDownload\tFailed", 
                    "!TickerSymbol\tLastDownload\tFailed", 
                    "EDGE.ST\t2017-02-18 09:00:00\t", 
                    "SHOT.ST\t2017-02-18 09:00:00\t", 
                });

            IList<string> result = new List<string>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => { result = y; });

            var data_file = TeaboxDataFile.Open(file.Object);
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("// TickerSymbol\tLastDownload\tFailed"));
            Assert.That(result[1], Is.EqualTo("!TickerSymbol\tLastDownload\tFailed"));
            Assert.That(result[2], Is.EqualTo("EDGE.ST\t2017-02-18 09:00:00\t"));
            Assert.That(result[3], Is.EqualTo("SHOT.ST\t2017-02-18 09:00:00\t"));
        }

        /*[Test, Explicit]
        public void CodeForIdentifingCorrectlyMarkedProperties()
        {
            var foo = new FileRowItem { };
            foreach (var prop in foo.GetType().GetProperties())
            {
                Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));

                var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), false);

                if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                    Console.WriteLine("TeaboxDataAttribute: Yes");
                else
                    Console.WriteLine("TeaboxDataAttribute: No");

                if (prop.PropertyType == typeof(string))
                    Console.WriteLine("Type: string");
                if (prop.PropertyType == typeof(int))
                    Console.WriteLine("Type: int");
                if (prop.PropertyType == typeof(DateTime))
                    Console.WriteLine("Type: DateTime");
            }
        }*/

    }

}


