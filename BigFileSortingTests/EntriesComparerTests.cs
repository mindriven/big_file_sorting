using FluentAssertions;

namespace BigFileSortingTests;

public class EntriesComparerTests
{

    private EntriesComparer comparer = new EntriesComparer();
    [Theory]
    [InlineData(0, "Apple", 0, "Apple", 0)]
    [InlineData(0, "Apple", 0, "Banana", -1)]
    [InlineData(0, "Banana", 0, "Apple", 1)]
    [InlineData(10, "Apple", 0, "Apple", 1)]
    [InlineData(10, "Apple", 10, "Apple", 0)]
    [InlineData(10, "Apple", 20, "Apple", -1)]
    public void ComparesCorrectly(int xInt, string xString, int yInt, string yString, int expected)
    {
        comparer.Compare(new(xInt, xString), new(yInt, yString)).Should().Be(expected);
    }

    [Fact]
    public void CanBeUsedToSortCollection()
    {
        var collection = new List<(int, string)>(){
            (415, "Apple"),
            (30432, "Something something something"),
            (1, "Apple"),
            (32, "Cherry is the best"),
            (2, "Banana is yellow")
        };

        collection.Order(comparer).Should().HaveCount(5).
            And.ContainInConsecutiveOrder(new[] {
                (1, "Apple"),
                (415, "Apple"),
                (2, "Banana is yellow"),
                (32, "Cherry is the best"),
                (30432, "Something something something"),
                });
    }
}