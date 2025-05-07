
using Funkin.Data;
using Funkin.Data.Versions.Latest;
using Funkin.Utils.Interfaces;

namespace Tests
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var metadata = Converter.DeserializeMetadata(File.ReadAllText("./metadata.json"));
            if (metadata is IConvertible<Metadata> convertible)
            {
                convertible.TryConvert(out var newMetadata);
                Console.WriteLine($"Old Metadata: {metadata}");
                Console.WriteLine($"New Metadata: {newMetadata}");
            }
        }
    }
}