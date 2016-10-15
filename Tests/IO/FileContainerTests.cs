using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using TeaboxDataFormat.IO;

namespace TeaboxDataFormat.Tests.IO
{
    [TestFixture]
    public class FileContainerTests
    {
        [Test]
        public void CanReadAllLines()
        {
            string test_file_name = ".\\FileContainerTests_11.txt";

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);

            File.WriteAllLines(test_file_name, new List<string>() { "Hello", "World" });

            var file_container = new FileContainer(test_file_name);

            var result = file_container.ReadAllLines();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("Hello"));
            Assert.That(result[1], Is.EqualTo("World"));

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);
        }

        [Test]
        public void CanWriteAllLines()
        {
            string test_file_name = ".\\FileContainerTests_12.txt";

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);

            File.WriteAllLines(test_file_name, new List<string>() { "Old", "Stuff" });

            var file_container = new FileContainer(test_file_name);

            file_container.WriteAllLines(new List<string>() { "Hello", "World" });

            var result = File.ReadAllLines(test_file_name);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("Hello"));
            Assert.That(result[1], Is.EqualTo("World"));

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);
        }

        [Test, ExpectedException]
        public void WillFailIfFileIsMissingWhenReading()
        {
            string test_file_name = ".\\FileContainerTests_13.txt";

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);

            var file_container = new FileContainer(test_file_name);

            var result = file_container.ReadAllLines();
        }

        [Test]
        public void WontFailIfFileIsMissingWhenWriting()
        {
            string test_file_name = ".\\FileContainerTests_14.txt";

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);

            var file_container = new FileContainer(test_file_name);

            file_container.WriteAllLines(new List<string>());

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);
        }

        [Test]
        public void WillCreateIfMissing()
        {
            string test_file_name = ".\\FileContainerTests_15.txt";

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);

            var file_container = new FileContainer(test_file_name, true);

            var result = file_container.ReadAllLines();

            Assert.That(result.Count, Is.EqualTo(0));

            file_container.WriteAllLines(new List<string>() { "Hello", "World" });

            Assert.That(File.Exists(test_file_name));

            if (File.Exists(test_file_name))
                File.Delete(test_file_name);
        }
    }
}
