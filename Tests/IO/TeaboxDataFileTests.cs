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

            var data_file = TeaboxDataFile.DataTableToFile(data_table, file.Object);
            data_file.Save();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("!Brick\tColor\tAmount"));
            Assert.That(result[1], Is.EqualTo("3001\tRed\t18"));
            Assert.That(result[2], Is.EqualTo("3004\tBlue\t22 //Data line #2"));

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

        // ToDo: White space not preserved test
        // Comment in title perserved
        // Can save new object with values in correct columns
        // Write line with more data than titles???

    }
}
