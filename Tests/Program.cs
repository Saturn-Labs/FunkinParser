
using Funkin.Data;
using Funkin.Data.Versions.Latest;
using Funkin.Data.Versions.Latest.Chart;
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
                var newMetadata = convertible.Convert();
                Console.WriteLine($"Old Metadata: {metadata}");
                Console.WriteLine($"New Metadata: {newMetadata}");
            }

            var (meta, chart) = new Funkin.Data.Versions.v100.Chart.ChartData().Convert();
        }
    }
}