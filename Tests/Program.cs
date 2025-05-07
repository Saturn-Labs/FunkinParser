
using Funkin.Data;

namespace Tests;

internal static class Program
{
    private static void Main(string[] args)
    {
        var metadata = Converter.DeserializeMetadata(File.ReadAllText("./metadata.json"));
        Console.WriteLine(metadata);
        var casted = (Funkin.Data.Latest.Metadata)metadata;
        casted.TimeChanges[0].TimeStamp = 487;
        Console.WriteLine(Converter.SerializeMetadata(casted));
        Console.ReadKey();
    }
}