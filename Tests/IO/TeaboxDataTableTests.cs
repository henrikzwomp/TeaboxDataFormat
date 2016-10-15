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
    }
}
