using FluentAssertions;

namespace BigFileSortingTests;

public class PartsMergerTests
{
    [Fact]
    public void CanMergeAndSortFiles(){

        File.WriteAllLines("0_bigFile.lines.part", new[]{
            "415. Apple",
            "30432. Something something something"
        });

        File.WriteAllLines("1_bigFile.lines.part", new[]{
            "1. Apple",
            "2. Banana is yellow",
        });

        File.WriteAllLines("2_bigFile.lines.part", new[]{
            "32. Cherry is the best"
        });

        PartsMerger.MergePartsSorting(new []{
                "0_bigFile.lines.part",
                "1_bigFile.lines.part",
                "2_bigFile.lines.part"},
            "bigFile.lines.sorted");

        File.ReadAllLines("bigFile.lines.sorted").Should().HaveCount(5).And.ContainInConsecutiveOrder(new[]{
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        });

        File.Delete("0_bigFile.lines.part");
        File.Delete("1_bigFile.lines.part");
        File.Delete("2_bigFile.lines.part");
        File.Delete("bigFile.lines.sorted");
    }
}