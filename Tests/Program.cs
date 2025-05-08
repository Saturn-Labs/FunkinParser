
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
            var legacyChart = Converter.DeserializeChartData(File.ReadAllText("./legacy-chart.json"));
            if (legacyChart is IConvertible<Metadata, ChartData> convertible)
            {
                var (meta, chart) = convertible.Convert();
                Console.WriteLine($"Legacy Chart: {legacyChart}");
                Console.WriteLine($"New Metadata: {meta}");
                Console.WriteLine($"New Chart: {chart}");
                Console.WriteLine($"Converted Metadata: {Converter.Serialize(meta)}");
                Console.WriteLine($"Converted Chart: {Converter.Serialize(chart)}");
            }
        }
    }
}