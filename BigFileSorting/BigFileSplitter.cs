public class BigFileSplitter
{
    /// <summary>
    /// Splits input file into multiple smaller, sorted files. Returns paths to created .part files
    /// </summary>
    public static IEnumerable<string> SplitToSortedChunksOfSize(string sourcefilePath, string workDirPath, int howManyLinesPerFile = 1000)
    {
        List<string> result = new List<string>();
        // TODO validate path parameters
        // TODO refactor to reactive with File.ReadLinesAsync(sourcefilePath) and https://www.nuget.org/packages/System.Linq.Async
        var buffer = new string[howManyLinesPerFile];
        using (var sourceStream = File.Open(sourcefilePath, FileMode.Open))
        using (var reader = new StreamReader(sourceStream))
        {
            var linesCounter = 0;
            var flashesCounter = 0;
            while (!reader.EndOfStream)
            {
                buffer[linesCounter] = reader.ReadLine()!;
                linesCounter++;
                if (linesCounter == howManyLinesPerFile)
                {
                    var partFileName = getPartFileName(workDirPath, flashesCounter, sourcefilePath);
                    flushSorted(buffer, howManyLinesPerFile, partFileName);
                    flashesCounter++;
                    linesCounter = 0;
                    result.Add(partFileName);
                }
            }
            if (linesCounter > 0)
            {
                var partFileName = getPartFileName(workDirPath, flashesCounter, sourcefilePath);
                flushSorted(buffer, linesCounter, partFileName);
                result.Add(partFileName);
            }
        }
        return result;
    }

    private static void flushSorted(IEnumerable<string> buffer, int howManyLines, string targetFilePath)
    {
        IComparer<(int, string)> comparer = new EntriesComparer();
        var tuples = buffer
                        .Take(howManyLines)
                        .Select(x => x.ToEntry())
                        .ToList();
        tuples.Sort(comparer);
        var lines = tuples.Select(t => t.ToLine());

        File.WriteAllLines(targetFilePath, lines);
    }

    private static string getPartFileName(string workDirPath, int counter, string sourceFilePath)
    {
        return Path.Combine(workDirPath, $"{counter}_{Path.GetFileName(sourceFilePath)}.part");
    }
}