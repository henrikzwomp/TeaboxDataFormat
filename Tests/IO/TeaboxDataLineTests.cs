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
    public class TeaboxDataLineTests
    {
        [Test]
        public void CanSetNonDefinedFields()
        {
            var line = new TeaboxDataLine();

            Assert.That(TeaboxDataLine.GetData(line).Count, Is.EqualTo(0));

            TeaboxDataLine.SetData(line, 0, "Test 1");

            Assert.That(TeaboxDataLine.GetData(line).Count, Is.EqualTo(1));

            TeaboxDataLine.SetData(line, 3, "Test 2");

            Assert.That(TeaboxDataLine.GetData(line).Count, Is.EqualTo(4));
        }

        [Test]
        public void SettingDataWithReadOnlyCollectionDontPreventAddingNewFields()
        {
            string[] data = new string[1] { "Hello" };

            var line = new TeaboxDataLine();

            TeaboxDataLine.SetData(line, data);

            TeaboxDataLine.SetData(line, 1, "World");

            Assert.That(TeaboxDataLine.GetData(line, 0), Is.EqualTo("Hello"));
            Assert.That(TeaboxDataLine.GetData(line, 1), Is.EqualTo("World"));
        }

        [Test]
        public void SettingTitleWithReadOnlyCollectionDontPreventAddingNewFields()
        {
            string[] title = new string[1] { "Hello" };

            var line = new TeaboxDataLine();

            TeaboxDataLine.SetTitles(line, title);

            Assert.That(TeaboxDataLine.GetData(line, "Hello", "Def"), Is.EqualTo("Def"));

            TeaboxDataLine.SetData(line, "World", "Test");

            Assert.That(TeaboxDataLine.GetData(line, "Hello", "Def"), Is.EqualTo("")); // Adding new value set "" on column before new
            Assert.That(TeaboxDataLine.GetData(line, "World", "Def"), Is.EqualTo("Test"));
        }

        [Test]
        public void CanCompareWithEqualsMethod()
        {
            var item1 = new TeaboxDataLine();
            var item2 = new TeaboxDataLine();

            Assert.That(item1.Equals(item1), Is.True);
            Assert.That(item1.Equals(item2), Is.False);
            Assert.That(item1.Equals("Something"), Is.False);
        }
    }
}
