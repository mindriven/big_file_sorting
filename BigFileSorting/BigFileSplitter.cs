public class BigFileSplitter
{
    // static IComparer() comparer = new EntriesComparer();

    public static void SplitToSortedChunksOfSize(string sourcefilePath, string workDirPath, int howManyLinesPerFile = 1000)
    {
        // TODO path validate parameters
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
                    flushSorted(buffer, howManyLinesPerFile, getPartFileName(workDirPath, flashesCounter, sourcefilePath));
                    flashesCounter++;
                    linesCounter = 0;
                }
            }
            if (linesCounter > 0)
                flushSorted(buffer, linesCounter, getPartFileName(workDirPath, flashesCounter, sourcefilePath));
        }
    }

    private static void flushSorted(IEnumerable<string> buffer, int howManyLines, string targetFilePath)
    {
        IComparer<(int, string)> comparer = new EntriesComparer();
        var tuples = buffer
                        .Take(howManyLines)
                        .Select(x=>x.ToEntry())
                        .ToList();
        tuples.Sort(comparer);
        var lines = tuples.Select(t=>t.ToLine());
        
        File.WriteAllLines(targetFilePath, lines);
    }

    private static string getPartFileName(string workDirPath, int counter, string sourceFilePath)
    {
        return Path.Combine(workDirPath, $"{counter}_{Path.GetFileName(sourceFilePath)}.part");
    }
}