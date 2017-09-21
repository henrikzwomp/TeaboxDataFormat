using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using TeaboxDataFormat.IO;

namespace TeaboxDataFormat.Tests.IO
{
    [TestFixture]
    public class TeaboxDataTableTests
    {

        [Test]
        public void CanAddRow()
        {
            var table = new TeaboxDataTable("Test");
            table.Add(new TeaboxDataRow());
        }

        [Test]
        public void CanGetCount()
        {
            var table = new TeaboxDataTable("Test");

            Assert.That(table.Count, Is.EqualTo(0));

            table.Add(new TeaboxDataRow());

            Assert.That(table.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanGetRowWithIndexNumber()
        {
            var table = new TeaboxDataTable("Test");
            
            var row = new TeaboxDataRow();
            row["Test"] = "Hello World";

            table.Add(row);

            Assert.That(table.Count, Is.EqualTo(1));
            Assert.That(table[0]["Test"], Is.EqualTo("Hello World"));
        }

        [Test]
        public void CanEnumarate()
        {
            var table = new TeaboxDataTable("Test");

            var row1 = new TeaboxDataRow(); row1["Test"] = "Hello ";
            var row2 = new TeaboxDataRow(); row2["Test"] = "World";
            var row3 = new TeaboxDataRow(); row3["Test"] = "!!!";

            table.Add(row1);
            table.Add(row2);
            table.Add(row3);

            Assert.That(table.Count, Is.EqualTo(3));
            string result = "";

            foreach(var row in table)
            {
                result += row[0];
            }

            Assert.That(result, Is.EqualTo("Hello World!!!"));
        }

        /*[Test]
        public void CanTurnDataTableToFile()
        {
            IList<string> result = new List<string>();
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback((IList<string> y) => result = y);

            var data_table = new TeaboxDataTable("Brick", "Color", "Amount");

            var brick_3001 = new TeaboxDataRow();
            brick_3001["Brick"] = "3001";
            brick_3001["Color"] = "Red";
            brick_3001["Amount"] = "18";

            var brick_3003 = new TeaboxDataRow();
            TeaboxDataLine.SetData(brick_3003, "Brick", "3004");
            TeaboxDataLine.SetData(brick_3003, "Color", "Blue");
            TeaboxDataLine.SetData(brick_3003, "Amount", "22");

            TeaboxDataLine.SetComment(brick_3003, "Data line #2");

            data_table.Add(brick_3001);
            data_table.Add(brick_3003);

            var data_file = data_table.ToTeaboxDataFile(file.Object);
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("!Brick\tColor\tAmount"));
            Assert.That(result[1], Is.EqualTo("3001\tRed\t18"));
            Assert.That(result[2], Is.EqualTo("3004\tBlue\t22 //Data line #2"));

        }*/
    }
}
