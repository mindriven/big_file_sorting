namespace BigFileSorting;
public class FileGenerator
{

    private static string[] fruits = new[] { "Banana", "Apple", "Orange", "Strawberry", "Papaya", "Mango", "Cherry", "Kiwi" };

    private static string[] adjectives = new[] { "tasty", "soft", "hard", "nice", "sweet", "sour", "red", "green", "rotten", "toxic", "healthy" };

    private static Random rnd = new Random();

    public static async Task Generate(int howManyLines, string targetFilePath, int batchSize = 500)
    {
        var howManyBatches = howManyLines / batchSize;
        //performance choice was to do it in parallel or async. Since writing to disk is much slower
        //than generation, chosen await
        await Enumerable.Range(0, howManyBatches + 1).ToList().ForEachAsync(async batchNumber =>
        {
            var howManyLinesInThisBatch = Math.Min(howManyLines - batchNumber * batchSize, batchSize);
            var bytes = generateBatchBytes(howManyLinesInThisBatch);
            using var targetStream = File.Open(targetFilePath, FileMode.OpenOrCreate);
            targetStream.Seek(0, SeekOrigin.End);
            await targetStream.WriteAsync(bytes, 0, bytes.Length);
            
        });
    }

    private static byte[] generateBatchBytes(int howManyLines)
    {
        var lines = Enumerable.Range(0, howManyLines)
                              .Select(x => @$"{rnd.Next()}. {fruits[rnd.Next(fruits.Length)]} is {adjectives[rnd.Next(adjectives.Length)]}" + Environment.NewLine);

        var bytes = lines.Select(x => System.Text.Encoding.UTF8.GetBytes(x))
                         .SelectMany(x => x)
                         .ToArray();
        return bytes;
    }
}