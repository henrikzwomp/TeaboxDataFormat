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
    public class TeaboxDataRowTests
    {
        [Test]
        public void TryingToGetNonExistingTitleWillReturnEmptyString()
        {
            var row = new TeaboxDataRow();
            Assert.That(TeaboxDataLine.GetData(row, "Test"), Is.EqualTo(""));
        }

        [Test]
        public void TryingToGetNonExistingValueWillReturnEmptyString() // Title exists
        {
            var row = new TeaboxDataRow();
            TeaboxDataLine.SetTitles(row, new List<string>() { "Test" });
            Assert.That(TeaboxDataLine.GetData(row, "Test"), Is.EqualTo(""));
        }

        [Test]
        public void CanSetValueToColumn()
        {
            var row = new TeaboxDataRow();
            Assert.That(TeaboxDataLine.GetData(row, "Test"), Is.EqualTo(""));

            TeaboxDataLine.SetData(row, "Test", "Hello World");

            Assert.That(TeaboxDataLine.GetData(row, "Test"), Is.EqualTo("Hello World"));
        }

        [Test]
        public void RowIsSetAsData()
        {
            var row = new TeaboxDataRow();
            Assert.That(TeaboxDataLine.GetLineType(row), Is.EqualTo(TeaboxDataLineType.Data));
        }
    }
}
