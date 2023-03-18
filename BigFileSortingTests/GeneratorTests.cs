using System.Text.RegularExpressions;
using BigFileSorting;
using FluentAssertions;

namespace BigFileSortingTests;

public class GeneratorTest
{
    [Fact]
    public async Task CanGenerateEmptyFileAsync()
    {
        const string targetFilePath = "./0.lines";
        await FileGenerator.Generate(0, targetFilePath);

        File.ReadAllLines(targetFilePath).Count().Should().Be(0);

        File.Delete(targetFilePath);
    }

    [Fact]
    public async Task CanGenerateFileWith10ProperLinesAsync()
    {
        const string targetFilePath = "./10.lines";
        await FileGenerator.Generate(10, targetFilePath);

        var allLines = File.ReadAllLines(targetFilePath);
        allLines.Count().Should().Be(10);
        allLines.Should().OnlyContain(x=>Regex.IsMatch(x, @"^\d+.(\s.+)$"));

        File.Delete(targetFilePath);
    }


    [Theory]
    [InlineData(2000, 200)]
    [InlineData(1998, 500)]
    [InlineData(2022, 500)]
    public async void CanGenerateBigFileInBatchesWithSomeStringRepetition(int howManyLines, int howManyBatches)
    {
        string targetFilePath = @$"./{howManyLines}.lines";
        await FileGenerator.Generate(howManyLines, targetFilePath, howManyBatches);

        var allLines = File.ReadAllLines(targetFilePath);
        allLines.Count().Should().Be(howManyLines);
        allLines.Select(x=>x.Split('.')[1]).Distinct().Count().Should().BeLessThan(2000);

        File.Delete(targetFilePath);
    }


    /// This test illustrates that generator can be used to generate
    /// amount of lines exceeding int.MaxValue by sequential execution
    [Fact]
    public async void SequentialExecutionAppendsToFile()
    {
        string targetFilePath = "./10.lines";
        await FileGenerator.Generate(5, targetFilePath);
        await FileGenerator.Generate(5, targetFilePath);

        var allLines = File.ReadAllLines(targetFilePath);
        allLines.Count().Should().Be(10);

        File.Delete(targetFilePath);
    }
}