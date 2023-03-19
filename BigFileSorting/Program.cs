// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BigFileSorting;

var me = Process.GetCurrentProcess();
var cliArgs = Environment.GetCommandLineArgs();
if (cliArgs.Length != 4 || (cliArgs[1] != "generate" && cliArgs[1] != "sort"))
{
    Console.WriteLine("Do one of those:");
    Console.WriteLine("- generate <<path>> <<noOnLines>> - generates file with lines");
    Console.WriteLine("- sort <<path>> <<chunkSize>> - sorts the file, using directory of the file as workspace");
}

Stopwatch stopwatch = new Stopwatch();
if (cliArgs[1] == "generate")
{
    stopwatch.Start();
    await FileGenerator.Generate(int.Parse(cliArgs[3]), cliArgs[2]);
    stopwatch.Stop();
    Console.WriteLine($"Done in {stopwatch.Elapsed}");
}

if (cliArgs[1] == "sort")
{
    var workdir = Path.GetDirectoryName(cliArgs[2]);
    stopwatch.Start();
    var partsToMerge = BigFileSplitter.SplitToSortedChunksOfSize(cliArgs[2], workdir, int.Parse(cliArgs[3]));
    Console.WriteLine($"Done splitting in {stopwatch.Elapsed}");
    PartsMerger.MergePartsSorting(partsToMerge, Path.Combine(workdir, "result.sorted"));
    stopwatch.Stop();
    Console.WriteLine($"Done in {stopwatch.Elapsed}");
}