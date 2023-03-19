public class EntriesComparer : IComparer<(int, string)>
{
    public int Compare((int, string) x, (int, string) y)
    {
        var stringComparisonResult = String.Compare(x.Item2, y.Item2, StringComparison.InvariantCulture);
        return (stringComparisonResult == 0)
                    ? x.Item1.CompareTo(y.Item1)
                    : stringComparisonResult;
    }
}