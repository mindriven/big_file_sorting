using BigFileSorting;
using FluentAssertions;

namespace BigFileSortingTests;

public class BigFileSplitterTests
{
    [Theory]
    [InlineData(1000, 100, 10, 100)]
    [InlineData(100, 100, 1, 100)]
    [InlineData(98, 50, 2, 48)]
    [InlineData(102, 50, 3, 2)]
    public async void SplitsToCorrectAmountOfFiles(int linesInBigFile, int linesPerPartFile, int expectedNoOfFiles, int expectedLinesNoInLastFile)
    {
        string bigFilePath = $"./{linesInBigFile}.lines";
        string workspaceDir = $"./workspace/{new Random().Next()}/";
        Directory.CreateDirectory(workspaceDir);
        await FileGenerator.Generate(linesInBigFile, bigFilePath);

        BigFileSplitter.SplitToSortedChunksOfSize(bigFilePath, workspaceDir, linesPerPartFile);

        var cratedFiles = Directory.GetFiles(workspaceDir, $"*_{linesInBigFile}.lines.part");
        cratedFiles.Count().Should().Be(expectedNoOfFiles);
        File.ReadAllLines(cratedFiles.Order().Last()).Should().HaveCount(expectedLinesNoInLastFile);

        Directory.GetFiles(workspaceDir).ToList().ForEach(f => File.Delete(f));
        File.Delete(bigFilePath);
        Directory.Delete(workspaceDir);
    }

    [Fact]
    public void SortsContentOfPartialFilesCase1()
    {
        const string bigFilePath = "bigFile.lines";
        if (File.Exists(bigFilePath)) File.Delete(bigFilePath);

        File.WriteAllLines("bigFile.lines", new[]{
            "415. Apple",
            "30432. Something something something",
            "1. Apple",
            "32. Cherry is the best",
            "2. Banana is yellow"
        });

        BigFileSplitter.SplitToSortedChunksOfSize(bigFilePath, "./", 3);

        File.ReadAllLines("0_bigFile.lines.part").Should().HaveCount(3).And.ContainInConsecutiveOrder(new[]{
            "1. Apple",
            "415. Apple",
            "30432. Something something something"
        });


        File.ReadAllLines("1_bigFile.lines.part").Should().HaveCount(2).And.ContainInConsecutiveOrder(new[]{
            "2. Banana is yellow",
            "32. Cherry is the best"
        });

        File.Delete(bigFilePath);
    }


    [Fact]
    public void SortsContentOfPartialFilesCase2()
    {
        const string bigFilePath = "bigFile.lines";
        if (File.Exists(bigFilePath)) File.Delete(bigFilePath);

        File.WriteAllLines("bigFile.lines", new[]{
            "30432. Something something something",
            "415. Apple",
            "2. Banana is yellow",
            "1. Apple",
            "32. Cherry is the best"
        });

        BigFileSplitter.SplitToSortedChunksOfSize(bigFilePath, "./", 2);

        File.ReadAllLines("0_bigFile.lines.part").Should().HaveCount(2).And.ContainInConsecutiveOrder(new[]{
            "415. Apple",
            "30432. Something something something"
        });

        File.ReadAllLines("1_bigFile.lines.part").Should().HaveCount(2).And.ContainInConsecutiveOrder(new[]{
            "1. Apple",
            "2. Banana is yellow",
        });

        File.ReadAllLines("2_bigFile.lines.part").Should().HaveCount(1).And.ContainInConsecutiveOrder(new[]{
            "32. Cherry is the best"
        });

        File.Delete(bigFilePath);
    }
};