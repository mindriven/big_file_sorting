using System.Text;
using BigFileSorting;

public class PartsMerger
{
    private static EntriesComparer comparer = new EntriesComparer();

    private static int writeBufferSize = 10000;
    public static void MergePartsSorting(IEnumerable<string> partFiles, string writeResultsTo)
    {
        var readers = partFiles.Select(path => new BufferedEntriesReader(path)).ToList();
        using (var targetStream = File.Open(writeResultsTo, FileMode.OpenOrCreate))
        using (StreamWriter sw = new StreamWriter(targetStream, Encoding.UTF8, writeBufferSize))
        {
            while (readers.Any(r => r.HasMoreLines))
            {
                var readerWithMinValue = readers.Where(r=>r.HasMoreLines).OrderBy(reader => reader.CurrentEntry, comparer).First();
                sw.WriteLine(readerWithMinValue.CurrentEntry.ToLine());
                readerWithMinValue.Next();
            }
        }

        readers.ForEach(r => r.Dispose());//TODO do disposable collection
    }
}

class BufferedEntriesReader : IDisposable
{
    public (int, string) CurrentEntry { get; private set; }

    private FileStream stream { get; set; }
    private StreamReader reader { get; set; }

    public BufferedEntriesReader(string path)
    {
        stream = File.Open(path, FileMode.Open);
        reader = new StreamReader(stream);
        this.Next();
    }

    public void Dispose()
    {
        reader.Dispose();
        stream.Dispose();
    }

    internal bool HasMoreLines { get; private set;} = true;

    internal void Next()
    {
        var line = reader.ReadLine();
        if (line == null) this.HasMoreLines = false;
        else
        {
            this.CurrentEntry = line.ToEntry();
        }

    }
}