internal static class Extensions
{
    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
        foreach (var value in list)
        {
            await func(value);
        }
    }

    public static string ToLine(this (int, string) entry) => entry.Item1 + "." + entry.Item2;

    public static (int, string) ToEntry(this string input)
    {
        var split = input.Split('.');
        return (int.Parse(split[0]), split[1]);
    }
}