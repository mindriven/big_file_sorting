using System.Text.RegularExpressions;
using BigFileSorting;
using FluentAssertions;

namespace BigFileSortingTests;

public class GeneratorTest
{
    [Fact]
    public void CanGenerateEmptyFile()
    {
        const string targetFilePath = "./0.lines";
        FileGenerator.Generate(0, targetFilePath);

        File.ReadAllLines(targetFilePath).Count().Should().Be(0);

        File.Delete(targetFilePath);
    }

    [Fact]
    public void CanGenerateFileWith10ProperLines()
    {
        const string targetFilePath = "./10.lines";
        FileGenerator.Generate(10, targetFilePath);

        var allLines = File.ReadAllLines(targetFilePath);
        allLines.Count().Should().Be(10);
        allLines.Should().OnlyContain(x=>Regex.IsMatch(x, @"^\d+.(\s.+)$"));

        File.Delete(targetFilePath);
    }


    [Fact]
    public void CanGenerateBigFileWithSomeStringRepetition()
    {
        const string targetFilePath = "./2000.lines";
        FileGenerator.Generate(2000, targetFilePath);

        var allLines = File.ReadAllLines(targetFilePath);
        allLines.Count().Should().Be(2000);
        allLines.Select(x=>x.Split('.')[1]).Distinct().Count().Should().BeLessThan(2000);

        File.Delete(targetFilePath);
    }
}