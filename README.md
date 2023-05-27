# TeaboxDataFormat

Library for reading and writing tab separated text data.

## About

I found myself writing the same code for read and saving text data to files over and over again for various small projects, so I started this project have a package I could reuse instead.

My requirements for this project was
- Easy to read and modify in text or spreadsheet editor. There should be no need for dedicate reader/editor for this format.
- Allow for comments and white space.
- Liberal in how data is ordered and forgiving of missing data.

As it has been implemented in a few various personal project.

Note that this library is still a work in process and things might change.

## File Structure & usage example

Below is an example of how a file can look.
```
// Double slashes starts a comment that will be ignored 
!Brick	Color	Amount // Line that starts with ! names the columns (not required)
3001	Red	18 // Data is separated by tabs
3004	Blue	22 // Comment must have space or tab infront of it if line also includes data. Otherwise it will be considered part of data.
```

It can be read in the following way. 

```
var data_file = TeaboxDataFile.Open<ProjectSpecificItemClass>("X:\\Sample.txt");

foreach(TeaboxDataRow row in data_table)
{
    Console.WriteLine("Part: " + row.Brick);
    Console.WriteLine("Color: " + row.Color);
    Console.WriteLine("Amount: " + row.Amount);
}
```

The class ProjectSpecificItemClass would look like this in code.

```
public class ProjectSpecificItemClass : TeaboxDataLine
{
    public string Brick { get; set; }
    public string Color { get; set; }
    public int Amount { get; set; }
}
```

Classes in this library are meant to be extended in project specific child classes. Which is the reason some functionality is hidden away in protected methods.

## Requirements / Dependencies
- .Net Framework 4.8
- NUnit 
- Moq 
- Castle.Core
