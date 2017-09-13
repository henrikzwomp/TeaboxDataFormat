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
    public class TeaboxDataFileTests
    {
        [Test]
        public void CanParseSimpleDataFile()
        {
            var text_lines = new List<string>()
            {
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(2));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 0), Is.EqualTo("stuff.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 1), Is.EqualTo("11"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetData(data_file[1]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 0), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 1), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 2), Is.EqualTo("..."));

        }

        [Test]
        public void CanParseComments()
        {

            var text_lines = new List<string>()
            {
                "// This file is for...", 
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(4));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetComment(data_file[0]), Is.EqualTo(" This file is for..."));
            Assert.That(TeaboxDataLine.GetData(data_file[1]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 0), Is.EqualTo("stuff.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 1), Is.EqualTo("11"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetData(data_file[2]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 0), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 1), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetComment(data_file[2]), Is.EqualTo(" my comment"));
            Assert.That(TeaboxDataLine.GetData(data_file[3]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetComment(data_file[3]), Is.EqualTo("stuff3.txt\t11\t..."));
        }

        [Test]
        public void CanSortOutData()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...", 
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);
            var data = data_file.GetData();

            Assert.That(data.Count, Is.EqualTo(2));
            Assert.That(TeaboxDataLine.GetData(data[0]).Count, Is.EqualTo(3));
            Assert.That(data[0][0], Is.EqualTo("stuff.txt"));
            Assert.That(data[0][1], Is.EqualTo("11"));
            Assert.That(data[0][2], Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetData(data[1]).Count, Is.EqualTo(3));
            Assert.That(data[1][0], Is.EqualTo("stuff2.txt"));
            Assert.That(data[1][1], Is.EqualTo("17"));
            Assert.That(data[1][2], Is.EqualTo("..."));
        }

        [Test]
        public void WillTrimData()
        {

            var text_lines = new List<string>()
            {
                "stuff2.txt \t17\t... // my comment",
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(1));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 0), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 1), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 2), Is.EqualTo("..."));
        }

        [Test]
        public void CanHandleEmptyLines()
        {

            var text_lines = new List<string>()
            {
                "stuff.txt\t11\t...", 
                "", 
                " ",
                "stuff2.txt\t17\t...",
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(4));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 0), Is.EqualTo("stuff.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 1), Is.EqualTo("11"));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetLineType(data_file[0]), Is.EqualTo(TeaboxDataLineType.Data));
            Assert.That(TeaboxDataLine.GetData(data_file[1]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetLineType(data_file[1]), Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(TeaboxDataLine.GetData(data_file[2]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetLineType(data_file[2]), Is.EqualTo(TeaboxDataLineType.Other));
            Assert.That(TeaboxDataLine.GetData(data_file[3]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 0), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 1), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetLineType(data_file[3]), Is.EqualTo(TeaboxDataLineType.Data));
        }

        [Test]
        public void CanGetDataWithTitles()
        {

            var text_lines = new List<string>()
            {
                "// This file is for...", 
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(5));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 0), Is.EqualTo(""));
            Assert.That(TeaboxDataLine.GetComment(data_file[0]), Is.EqualTo(" This file is for..."));
            Assert.That(TeaboxDataLine.GetLineType(data_file[0]), Is.EqualTo(TeaboxDataLineType.Other));

            Assert.That(TeaboxDataLine.GetData(data_file[1]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 0), Is.EqualTo("File"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 1), Is.EqualTo("Size"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 2), Is.EqualTo("Something"));
            Assert.That(TeaboxDataLine.GetLineType(data_file[1]), Is.EqualTo(TeaboxDataLineType.Titles));

            Assert.That(TeaboxDataLine.GetData(data_file[2]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 0), Is.EqualTo("stuff.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 1), Is.EqualTo("11"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetData(data_file[2], "File"), Is.EqualTo("stuff.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], "Size"), Is.EqualTo("11"));
            Assert.That(TeaboxDataLine.GetData(data_file[2], "Something"), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetLineType(data_file[2]), Is.EqualTo(TeaboxDataLineType.Data));

            Assert.That(TeaboxDataLine.GetData(data_file[3]).Count, Is.EqualTo(3));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 0), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 1), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], 2), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetData(data_file[3], "File"), Is.EqualTo("stuff2.txt"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], "Size"), Is.EqualTo("17"));
            Assert.That(TeaboxDataLine.GetData(data_file[3], "Something"), Is.EqualTo("..."));
            Assert.That(TeaboxDataLine.GetComment(data_file[3]), Is.EqualTo(" my comment"));
            Assert.That(TeaboxDataLine.GetLineType(data_file[3]), Is.EqualTo(TeaboxDataLineType.Data));

            Assert.That(TeaboxDataLine.GetData(data_file[4]).Count, Is.EqualTo(0));
            Assert.That(TeaboxDataLine.GetComment(data_file[4]), Is.EqualTo("stuff3.txt\t11\t..."));
            Assert.That(TeaboxDataLine.GetLineType(data_file[4]), Is.EqualTo(TeaboxDataLineType.Other));
        }

        [Test]
        public void MethodGetDataWillFilterOutLinesWithoutData()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...", 
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            var data = data_file.GetData();

            Assert.That(data.Titles.Length, Is.EqualTo(3));
            Assert.That(data.Titles[0], Is.EqualTo("File"));
            Assert.That(data.Titles[1], Is.EqualTo("Size"));
            Assert.That(data.Titles[2], Is.EqualTo("Something"));

            // Verify data
            Assert.That(data.Count, Is.EqualTo(2));
            Assert.That(TeaboxDataLine.GetData(data[0]).Count, Is.EqualTo(3));
            Assert.That(data[0][0], Is.EqualTo("stuff.txt"));
            Assert.That(data[0][1], Is.EqualTo("11"));
            Assert.That(data[0][2], Is.EqualTo("..."));
            Assert.That(data[0]["File"], Is.EqualTo("stuff.txt"));
            Assert.That(data[0]["Size"], Is.EqualTo("11"));
            Assert.That(data[0]["Something"], Is.EqualTo("..."));

            Assert.That(TeaboxDataLine.GetData(data[1]).Count, Is.EqualTo(3));
            Assert.That(data[1][0], Is.EqualTo("stuff2.txt"));
            Assert.That(data[1][1], Is.EqualTo("17"));
            Assert.That(data[1][2], Is.EqualTo("..."));
            Assert.That(data[1]["File"], Is.EqualTo("stuff2.txt"));
            Assert.That(data[1]["Size"], Is.EqualTo("17"));
            Assert.That(data[1]["Something"], Is.EqualTo("..."));
        }

        [Test]
        public void CanHandleSingleDataLines() // CanParseCibiData (Data from Cibi app.)
        {
            var text_lines = new List<string>()
            {
                "#Kai", 
                "1\t6113986\tx\t1 //Kai Hood",
                "1\t6057957\tx\t1 //Kai head", 
                "1\t6117084\tx\t1 //kai body",
                "1\t6117729\tx\t1 //Kai legs",
                "1\t6116639\tx\t1 //Sword Holder",
                "-- Can't Build"
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count(), Is.EqualTo(7));
            Assert.That(TeaboxDataLine.GetData(data_file[0]).Count, Is.EqualTo(1));
            Assert.That(TeaboxDataLine.GetData(data_file[0], 0), Is.EqualTo("#Kai"));
            Assert.That(TeaboxDataLine.GetData(data_file[1]).Count, Is.EqualTo(4));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 0), Is.EqualTo("1"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 1), Is.EqualTo("6113986"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 2), Is.EqualTo("x"));
            Assert.That(TeaboxDataLine.GetData(data_file[1], 3), Is.EqualTo("1"));
            Assert.That(TeaboxDataLine.GetComment(data_file[1]), Is.EqualTo("Kai Hood"));
        }

        [Test]
        public void CanWriteToFile()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...", 
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...", 
                "stuff2.txt\t17\t... // my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var was_called = false;
            IList<string> result = new List<string>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => { was_called = true; result = y; });

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Save();

            string expected = "// This file is for..." + Environment.NewLine +
                "!File\tSize\tSomething" + Environment.NewLine +
                "stuff.txt\t11\t..." + Environment.NewLine +
                "stuff2.txt\t17\t...// my comment" + Environment.NewLine +
                "//stuff3.txt\t11\t...";

            Assert.That(was_called, Is.True);

            Assert.That(result.Count, Is.EqualTo(text_lines.Count));
            Assert.That(result[0], Is.EqualTo(text_lines[0]));
            Assert.That(result[1], Is.EqualTo(text_lines[1]));
            Assert.That(result[2], Is.EqualTo(text_lines[2]));
            Assert.That(result[3], Is.EqualTo(text_lines[3]));
            Assert.That(result[4], Is.EqualTo(text_lines[4]));
        }
        
        [Test]
        public void CanWriteToFileLinesMissingSomeTitles()
        {
            var file = new Mock<IFileContainer>();

            var was_called = false;
            IList<string> result = new List<string>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => { was_called = true; result = y; });

            var test_file = new CanWriteToFileLinesMissingSomeTitles_TeaboxDataFile(new string[4] { "Title1", "Title2", "Title3", "Title4" });
            
            var line0 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line0, new List<string>() { "Title1", "Title2", "Title3", "Title4" } );
            TeaboxDataLine.SetTitles(line0, new List<string>() { "Title1", "Title2", "Title3", "Title4" } );
            TeaboxDataLine.SetLineType(line0, TeaboxDataLineType.Titles );
            TeaboxDataLine.SetComment(line0, "" );
            test_file.AddLine(line0);

            var line1 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line1, new List<string>() { "Value1", "Value2", "Value3", "Value4" });
            TeaboxDataLine.SetTitles(line1, new List<string>() { "Title1", "Title2", "Title3", "Title4" });
            TeaboxDataLine.SetLineType(line1, TeaboxDataLineType.Data);
            TeaboxDataLine.SetComment(line1, "");
            test_file.AddLine(line1);

            var line2 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line2, new List<string>() { "Value1" });
            TeaboxDataLine.SetTitles(line2, new List<string>() { "Title1" });
            TeaboxDataLine.SetLineType(line2, TeaboxDataLineType.Data);
            TeaboxDataLine.SetComment(line2, "");
            test_file.AddLine(line2);

            var line3 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line3, new List<string>() { "Value2", "Value4" });
            TeaboxDataLine.SetTitles(line3, new List<string>() { "Title2", "Title4" });
            TeaboxDataLine.SetLineType(line3, TeaboxDataLineType.Data);
            TeaboxDataLine.SetComment(line3, "");
            test_file.AddLine(line3);
            
            var line4 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line4, new List<string>() { "Value3" });
            TeaboxDataLine.SetTitles(line4, new List<string>() { "Title3" });
            TeaboxDataLine.SetLineType(line4, TeaboxDataLineType.Data);
            TeaboxDataLine.SetComment(line4, "");
            test_file.AddLine(line4);

            var line5 = new TeaboxDataLine();
            TeaboxDataLine.SetData(line5, new List<string>() { "Value4" });
            TeaboxDataLine.SetTitles(line5, new List<string>() { "Title4" });
            TeaboxDataLine.SetLineType(line5, TeaboxDataLineType.Data);
            TeaboxDataLine.SetComment(line5, "");
            test_file.AddLine(line5);

            // test
            test_file.CallWriteFile(file.Object);

            Assert.That(was_called, Is.True);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo("!Title1\tTitle2\tTitle3\tTitle4"));
            Assert.That(result[1], Is.EqualTo("Value1\tValue2\tValue3\tValue4"));
            Assert.That(result[2], Is.EqualTo("Value1\t\t\t"));
            Assert.That(result[3], Is.EqualTo("\tValue2\t\tValue4"));
            Assert.That(result[4], Is.EqualTo("\t\tValue3\t"));
            Assert.That(result[5], Is.EqualTo("\t\t\tValue4"));
        }

        public class CanWriteToFileLinesMissingSomeTitles_TeaboxDataFile : TeaboxDataFileBase<TeaboxDataLine>
        {
            public CanWriteToFileLinesMissingSomeTitles_TeaboxDataFile(string[] titles)
            {
                _lines = new List<TeaboxDataLine>();
                _titles = titles;
            }

            internal void AddLine(TeaboxDataLine new_line)
            {
                _lines.Add(new_line);
            }

            public void CallWriteFile(IFileContainer _file)
            {
                WriteFile(_file);
            }
        }

        [Test]
        public void TitlesSetBeforeFileIsBeingReadWillBeUsedInsteadOfAnyOnesFoundInFile()
        {
            var text_lines = new List<string>()
            {
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...",
                "stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var test1 = new TitlesSetBeforeFileIsBeingReadWillBeUsedInsteadOfAnyOnesFoundInFile_TeaboxDataFile(false);

            Assert.That(test1.Titles, Is.Null);

            test1.CallReadLines(file.Object);

            Assert.That(test1.Titles, Is.Not.Null);
            Assert.That(test1.Titles.Length, Is.EqualTo(3));
            Assert.That(test1.Titles[0], Is.EqualTo("File"));
            Assert.That(test1.Titles[1], Is.EqualTo("Size"));
            Assert.That(test1.Titles[2], Is.EqualTo("Something"));

            var test2 = new TitlesSetBeforeFileIsBeingReadWillBeUsedInsteadOfAnyOnesFoundInFile_TeaboxDataFile(true);

            Assert.That(test2.Titles, Is.Not.Null);

            Assert.That(test2.Titles, Is.Not.Null);
            Assert.That(test2.Titles.Length, Is.EqualTo(1));
            Assert.That(test2.Titles[0], Is.EqualTo("Title"));

            test2.CallReadLines(file.Object);

            Assert.That(test2.Titles, Is.Not.Null);
            Assert.That(test2.Titles.Length, Is.EqualTo(1));
            Assert.That(test2.Titles[0], Is.EqualTo("Title"));

        }

        public class TitlesSetBeforeFileIsBeingReadWillBeUsedInsteadOfAnyOnesFoundInFile_TeaboxDataFile : TeaboxDataFileBase<TeaboxDataLine>
        {
            public TitlesSetBeforeFileIsBeingReadWillBeUsedInsteadOfAnyOnesFoundInFile_TeaboxDataFile(bool set_title_in_constructor) // 
            {
                if (set_title_in_constructor)
                    _titles = new string[1] { "Title" };
            }

            public string[] Titles { get { return _titles; } }

            public void CallReadLines(IFileContainer file)
            {
                ReadFile(file);
            }
        }
        
        [Test]
        public void CanHandleNoTitlesWritingFileWithoutReadingItFirst()
        {
            IList<string> result = new List<string>();

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => result = y);

            var lines = new List<TeaboxDataRow>()
            {
                new TeaboxDataRow() { [0] = "Hello", [1] = "World" },
                new TeaboxDataRow() { [0] = "Some", [1] = "Thing" },
                new TeaboxDataRow() { [0] = "Strange", [1] = "Way" }
            };

            var writer = new CanHandleNoTitlesWritingFile_GenericFileWriter<TeaboxDataRow>();
            writer.Write(lines, file.Object);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("Hello\tWorld"));
            Assert.That(result[1], Is.EqualTo("Some\tThing"));
            Assert.That(result[2], Is.EqualTo("Strange\tWay"));
        }

        [Test]
        public void CanHandleNoTitlesWritingFileReadingItFirst()
        {
            IList<string> result = new List<string>();

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>() { "Hello\tWorld" });
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => result = y);

            var lines = new List<TeaboxDataRow>()
            {
                new TeaboxDataRow() { [0] = "Hello", [1] = "World" },
                new TeaboxDataRow() { [0] = "Some", [1] = "Thing" },
                new TeaboxDataRow() { [0] = "Strange", [1] = "Way" }
            };

            var writer = new CanHandleNoTitlesWritingFile_GenericFileWriter<TeaboxDataRow>();
            writer.Read(file.Object);
            writer.Write(lines, file.Object);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("Hello\tWorld"));
            Assert.That(result[1], Is.EqualTo("Some\tThing"));
            Assert.That(result[2], Is.EqualTo("Strange\tWay"));
        }
        
        public class CanHandleNoTitlesWritingFile_GenericFileWriter<output_type> : TeaboxDataFileBase<output_type> where output_type : TeaboxDataLine, new()
        {
            public void Read(IFileContainer file)
            {
                ReadFile(file);
            }

            public void Write(IEnumerable<output_type> items, IFileContainer file)
            {
                _lines = new List<output_type>(items);
                WriteFile(file);
            }
        }

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
        public void CanUpdateAndMergeData()
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

            data_file.UpdateAndMergeData<TestItemForGetDataAsWorksWithStringProperties>(data, "DateField1");
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("!DateField1\tDateField2"));
            Assert.That(result[1], Is.EqualTo("Hello\tMe"));
            Assert.That(result[2], Is.EqualTo("// A bit of a comment"));
            Assert.That(result[3], Is.EqualTo("Something\tWicked"));
        }

        [Test]
        public void WillAddNewRowsWhenMergingData()
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
            data.Add(new TestItemForGetDataAsWorksWithStringProperties() { DateField1 = "XX", DateField2 = "YY" });

            data_file.UpdateAndMergeData<TestItemForGetDataAsWorksWithStringProperties>(data, "DateField1");
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result[0], Is.EqualTo("!DateField1\tDateField2"));
            Assert.That(result[1], Is.EqualTo("Hello\tMe"));
            Assert.That(result[2], Is.EqualTo("// A bit of a comment"));
            Assert.That(result[3], Is.EqualTo("Something\tWicked"));
            Assert.That(result[4], Is.EqualTo("XX\tYY"));
        }

        [Test]
        public void CanOpenFileBasedOnATeaboxDataLineChildObject()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>() {});

            IList<string> result = new List<string>();
            file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>())).Callback<IList<string>>(y => { result = y; });

            var data_file = TeaboxDataFile.Open<TestItemForGetDataAsWorksWithStringProperties>(file.Object);
            var data = data_file.GetDataAs<TestItemForGetDataAsWorksWithStringProperties>();

            data.Add(new TestItemForGetDataAsWorksWithStringProperties() { DateField1 = "Hello", DateField2 = "World" });
            data.Add(new TestItemForGetDataAsWorksWithStringProperties() { DateField1 = "XX", DateField2 = "YY" });

            data_file.UpdateAndMergeData<TestItemForGetDataAsWorksWithStringProperties>(data, "DateField1");
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("!DateField1\tDateField2"));
            Assert.That(result[1], Is.EqualTo("Hello\tWorld"));
            Assert.That(result[2], Is.EqualTo("XX\tYY"));
        }

        [Test]
        public void BasedOnClassPropertyOrderWillNotChangeOrderInFile2()
        {
            var file_container = new Mock<IFileContainer>();
            file_container.Setup(x => x.ReadAllLines()).Returns(new List<string>() {
                "!FullName\tTickerSymbol\tGooglePrefix\tYahooSufix\tLastDownloadAttempt\tSuccessfulDownload\t",
                "Clas Ohlson AB\tCLAS-B\tSTO\tST\t2017-04-23 07:41\tTRUE\t",
                "Scandi Standard\tSCST\tSTO\tST\t",
            });

            var stock_list_file = TeaboxDataFile.Open<TestItemForBasedOnClassPropertyOrderWillNotChangeOrderInFile>(file_container.Object);
            var result = stock_list_file.GetDataAs<TestItemForBasedOnClassPropertyOrderWillNotChangeOrderInFile>();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].FullName, Is.EqualTo("Clas Ohlson AB"));
            Assert.That(result[0].TickerSymbol, Is.EqualTo("CLAS-B"));
            Assert.That(result[0].GooglePrefix, Is.EqualTo("STO"));
            Assert.That(result[0].YahooSufix, Is.EqualTo("ST"));
            Assert.That(result[0].LastDownloadAttempt, Is.EqualTo(new DateTime(2017, 4, 23, 7, 41, 0)));
            Assert.That(result[0].SuccessfulDownload, Is.EqualTo(true));

            Assert.That(result[1].FullName, Is.EqualTo("Scandi Standard"));
            Assert.That(result[1].TickerSymbol, Is.EqualTo("SCST"));
            Assert.That(result[1].GooglePrefix, Is.EqualTo("STO"));
            Assert.That(result[1].YahooSufix, Is.EqualTo("ST"));
            Assert.That(result[1].LastDownloadAttempt, Is.EqualTo(DateTime.MinValue));
            Assert.That(result[1].SuccessfulDownload, Is.EqualTo(false));
        }

        [Test]
        public void BasedOnClassPropertyOrderWillNotChangeOrderInFile()
        {
            var file_container = new Mock<IFileContainer>();
            file_container.Setup(x => x.ReadAllLines()).Returns(new List<string>() {
                "!FullName\tTickerSymbol\tGooglePrefix\tYahooSufix\tLastDownloadAttempt\tSuccessfulDownload\t",
                "Clas Ohlson AB\tCLAS-B\tSTO\tST\t2017-04-23 07:41\tTRUE\t",
                "Scandi Standard\tSCST\tSTO\tST\t",
            });

            var stock_list_file = TeaboxDataFile.Open<TestItemForBasedOnClassPropertyOrderWillNotChangeOrderInFile>(file_container.Object);
            var result = stock_list_file.GetData();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0]["FullName"], Is.EqualTo("Clas Ohlson AB"));
            Assert.That(result[0]["TickerSymbol"], Is.EqualTo("CLAS-B"));
            Assert.That(result[0]["GooglePrefix"], Is.EqualTo("STO"));
            Assert.That(result[0]["YahooSufix"], Is.EqualTo("ST"));
            Assert.That(result[0]["LastDownloadAttempt"], Is.EqualTo("2017-04-23 07:41"));
            Assert.That(result[0]["SuccessfulDownload"], Is.EqualTo("TRUE"));

            Assert.That(result[1]["FullName"], Is.EqualTo("Scandi Standard"));
            Assert.That(result[1]["TickerSymbol"], Is.EqualTo("SCST"));
            Assert.That(result[1]["GooglePrefix"], Is.EqualTo("STO"));
            Assert.That(result[1]["YahooSufix"], Is.EqualTo("ST"));
            Assert.That(result[1]["LastDownloadAttempt"], Is.EqualTo(""));
            Assert.That(result[1]["SuccessfulDownload"], Is.EqualTo(""));
        }

        public class TestItemForBasedOnClassPropertyOrderWillNotChangeOrderInFile : TeaboxDataLine
        {
            [TeaboxData]
            public string TickerSymbol { get; set; }
            [TeaboxData]
            public string GooglePrefix { get; set; }
            [TeaboxData]
            public string YahooSufix { get; set; }
            [TeaboxData]
            public string FullName { get; set; }
            [TeaboxData]
            public DateTime LastDownloadAttempt { get; set; }
            [TeaboxData]
            public bool SuccessfulDownload { get; set; }

            // ToDo Move
            [TeaboxData]
            public bool YahooDownloadFailed { get; set; }
            [TeaboxData]
            public string YahooDownloadFile { get; set; }
            [TeaboxData]
            public DateTime YahooDownloadDate { get; set; }
        }

        [Test]
        public void CanSaveWithNewCustomLineAdded()
        {
            IList<string> result = new List<string>();

            var setting_file = new Mock<IFileContainer>();

            setting_file.Setup(x => x.ReadAllLines()).Returns(new List<string>());
            setting_file.Setup(x => x.WriteAllLines(It.IsAny<IList<string>>()))
                .Callback<IList<string>>((y) => { result = y; });

            var _tbl_action_log_file = TeaboxDataFile.Open<TestItemForCanSaveWithNewCustomLineAdded>(setting_file.Object);

            var line = new TestItemForCanSaveWithNewCustomLineAdded();

            line.Action = "MyAction";
            line.Note = "MyNote";
            line.DbName = "MyDatabase";
            line.Message = "MyMessage";
            line.DateTime = DateTime.Now;
            line.ThreadId = 1;

            _tbl_action_log_file.Add(line);
            _tbl_action_log_file.Save();

            Assert.That(result.Count, Is.EqualTo(2));
            Console.WriteLine(result[0]);
            Console.WriteLine(result[1]);
            Assert.That(result[0], Is.EqualTo("!Action\tNote\tDbName\tMessage\tDateTime\tThreadId"));
            Assert.That(result[1].StartsWith("MyAction\tMyNote\tMyDatabase\tMyMessage\t"), Is.True);
        }

        private class TestItemForCanSaveWithNewCustomLineAdded : TeaboxDataLine
        {
            [TeaboxData]
            public string Action { get; set; }
            [TeaboxData]
            public string Note { get; set; }
            [TeaboxData]
            public string DbName { get; set; }
            [TeaboxData]
            public string Message { get; set; }
            [TeaboxData]
            public DateTime DateTime { get; set; }
            [TeaboxData]
            public int ThreadId { get; set; }
        }

        // ToDo: White space not preserved test
        // Comment in title perserved
        // Can save new object with values in correct columns
        // Write line with more data than titles???
        /*
            ToDo Fix so code can handled 3 annoying characters in the beginning of UTF8 text file.
            https://en.wikipedia.org/wiki/Byte_order_mark
            http://stackoverflow.com/questions/2223882/whats-different-between-utf-8-and-utf-8-without-bom
        */

        #region IList and ICollection tests
        [Test]
        public void CanGetAndSetLinesWithIndex()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(test_line_1);
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file[1], Is.EqualTo(test_line_1));

            var test_line_2 = new TeaboxDataLine();
            data_file[1] = test_line_2;

            Assert.That(data_file[1], Is.EqualTo(test_line_2));
        }

        [Test]
        public void CanUseIndexOfMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(test_line_1);
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(1));
        }

        [Test]
        public void CanUseInsertMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(-1));

            data_file.Insert(1, test_line_1);

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(1));
        }

        [Test]
        public void InsertMethodWillSetTitlesOnNewLine()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...",
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...",
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(new TeaboxDataLine());

            Assert.That(TeaboxDataLine.GetTitles(test_line_1).Count, Is.EqualTo(0));

            data_file.Insert(4, test_line_1);

            Assert.That(TeaboxDataLine.GetTitles(test_line_1).Count, Is.EqualTo(3));
        }

        [Test]
        public void CanUseRemoveAtMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(test_line_1);
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(1));

            data_file.RemoveAt(1);

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(-1));
        }

        [Test]
        public void CanUseAddMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(-1));
            data_file.Add(test_line_1);
            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(0));
        }

        [Test]
        public void AddMethodWillSetTitlesOnNewLine()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...",
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...",
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);


            Assert.That(TeaboxDataLine.GetTitles(test_line_1).Count, Is.EqualTo(0));

            data_file.Add(test_line_1);

            Assert.That(TeaboxDataLine.GetTitles(test_line_1).Count, Is.EqualTo(3));
        }

        [Test]
        public void CanUseClearMethod()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...",
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...",
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count, Is.EqualTo(5));

            data_file.Clear();

            Assert.That(data_file.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanUseContainsMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.Contains(test_line_1), Is.EqualTo(false));

            data_file.Add(test_line_1);

            Assert.That(data_file.Contains(test_line_1), Is.EqualTo(true));
        }

        [Test]
        public void CanUseCopyToMethod()
        {
            var text_lines = new List<string>()
            {
                "// This file is for...",
                "!File\tSize\tSomething",
                "stuff.txt\t11\t...",
                "stuff2.txt\t17\t...// my comment",
                "//stuff3.txt\t11\t..."
            };

            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(text_lines);

            var data_file = TeaboxDataFile.Open(file.Object);


            var lines = new TeaboxDataLine[5];

            data_file.CopyTo(lines, 0);

            Assert.That(TeaboxDataLine.GetData(lines[2],"File"), Is.EqualTo("stuff.txt"));
        }

        [Test]
        public void CanUseRemoveMethod()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var test_line_1 = new TeaboxDataLine();

            var data_file = TeaboxDataFile.Open(file.Object);

            data_file.Add(new TeaboxDataLine());
            data_file.Add(test_line_1);
            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(1));

            data_file.Remove(test_line_1);

            Assert.That(data_file.IndexOf(test_line_1), Is.EqualTo(-1));
        }

        [Test]
        public void CanUseCountProperty()
        {
            var file = new Mock<IFileContainer>();
            file.Setup(x => x.ReadAllLines()).Returns(new List<string>());

            var data_file = TeaboxDataFile.Open(file.Object);

            Assert.That(data_file.Count, Is.EqualTo(0));

            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.Count, Is.EqualTo(1));

            data_file.Add(new TeaboxDataLine());

            Assert.That(data_file.Count, Is.EqualTo(2));
        }

        #endregion
        
    }
}
