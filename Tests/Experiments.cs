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

        [Test, Explicit]
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

                if(prop.PropertyType == typeof(string))
                    Console.WriteLine("Type: string");
                if (prop.PropertyType == typeof(int))
                    Console.WriteLine("Type: int");
                if (prop.PropertyType == typeof(DateTime))
                    Console.WriteLine("Type: DateTime");
            }
        }
    }

    

    public class FileRowItem : TeaboxDataLine
    {
        //[TeaboxData]
        public int MyRowID { get; set; }
        [TeaboxData]
        public string MyRowData { get; set; }
        //[TeaboxData]
        public DateTime MyEditDate { get; set; }
    }
    
}


