namespace BigFileSorting;
public class FileGenerator
{

    private static string[] fruits = new[] { "Banana", "Apple", "Orange", "Strawberry", "Papaya", "Mango", "Cherry", "Kiwi" };

    private static string[] adjectives = new[] { "tasty", "soft", "hard", "nice", "sweet", "sour", "red", "green", "rotten", "toxic", "healthy" };

    public static void Generate(int howManyLines, string targetFilePath, int batchSize = 500)
    {
        Random rnd = new Random();
        var lines = Enumerable.Range(0, howManyLines)
                              .Select(x => @$"{rnd.Next()}. {fruits[rnd.Next(fruits.Length)]} is {adjectives[rnd.Next(adjectives.Length)]}"+Environment.NewLine);

        var bytes = lines.Select(x => System.Text.Encoding.UTF8.GetBytes(x))
                         .SelectMany(x=>x)
                         .ToArray();

        using (var targetStream = File.Open(targetFilePath, FileMode.Create))
        {
            targetStream.Seek(0, SeekOrigin.End);
            targetStream.Write(bytes, 0, bytes.Length);
        }
    }
}