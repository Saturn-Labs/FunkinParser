using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funkin.Core.Data;
using Funkin.Core.Data.Latest;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Core
{
    public class FunkinParser : IParser
    {
        public static Dictionary<VersionRange, Type> CompleteMetadataTypes { get; private set; } = new Dictionary<VersionRange, Type>();
        public static Dictionary<VersionRange, Type> CompleteChartDataTypes { get; private set; } = new Dictionary<VersionRange, Type>();

        public readonly string MetadataJsonText;
        public readonly string ChartJsonText;

        static FunkinParser()
        {
            AddMetadataType<SongData>(VersionRange.Parse("[2.0.0,2.1.0)"));
            AddMetadataType<Data.v21X.SongData>(VersionRange.Parse("[2.1.0,2.2.0)"));
            AddMetadataType<SongData>(VersionRange.Parse("[2.2.0,2.3.0)"));
            AddChartDataType<SongChartData>(VersionRange.Parse("[2.0.0,2.3.0)"));
            AddChartDataType<Data.v10X.SongChartData>(VersionRange.Parse("[1.0.0,2.0.0)"));
        }
        
        public FunkinParser(string metadataJsonText, string chartJsonText)
        {
            MetadataJsonText = metadataJsonText;
            ChartJsonText = chartJsonText;
        }
        
        public IData? ParseMetadata()
        {
            if (MetadataJsonText is null or { Length: 0 })
                return null;
            
            var partialMetadata = JsonConvert.DeserializeObject<VersioningData>(MetadataJsonText);
            if (partialMetadata is null)
                throw new NullReferenceException("Failed to deserialize metadata!");
            
            var type = CompleteMetadataTypes.FirstOrDefault(t => t.Key.Satisfies(partialMetadata.Version)).Value;
            if (type is null)
                throw new Exception($"Couldn't find metadata model for version '{partialMetadata.Version}'.");
            
            var instanceOfMetadata = JsonConvert.DeserializeObject(MetadataJsonText, type);
            return instanceOfMetadata as IData;
        }

        public IData? ParseChartData()
        {
            if (ChartJsonText is null or { Length: 0 })
                return null;
            
            var partialChartData = JsonConvert.DeserializeObject<VersioningData>(ChartJsonText);
            if (partialChartData is null)
                throw new NullReferenceException("Failed to deserialize chart data!");
            
            var type = CompleteChartDataTypes.FirstOrDefault(t => t.Key.Satisfies(partialChartData.Version)).Value;
            if (type is null)
                throw new Exception($"Couldn't find chart data model for version '{partialChartData.Version}'.");
            
            var instanceOfChartData = JsonConvert.DeserializeObject(ChartJsonText, type);
            return instanceOfChartData as IData;
        }

        public ISongPack<VersioningData, VersioningData>? Parse()
        {
            var metadata = ParseMetadata() as VersioningData;
            var chartData = ParseChartData() as VersioningData;
            return new SongPack<VersioningData, VersioningData>(metadata, chartData);
        }

        public static void AddMetadataType<T>(VersionRange version)
        {
            AddMetadataType(version, typeof(T));
        }
        
        public static void AddMetadataType(VersionRange versionRange, Type type)
        {
            CompleteMetadataTypes[versionRange] = type;
        }
        
        public static void AddChartDataType<T>(VersionRange version)
        {
            AddChartDataType(version, typeof(T));
        }
        
        public static void AddChartDataType(VersionRange versionRange, Type type)
        {
            CompleteChartDataTypes[versionRange] = type;
        }

        public static SongChartData? ConvertToLatestChartData(VersioningData data)
        {
            if (data is not (SongChartData or Data.v10X.SongChartData))
                return null;
            
            var latestMetadata = data;
            while (latestMetadata.GetType().GetInterfaces().FirstOrDefault(i => i.Name.StartsWith("IVersionConvertible")) != null)
            {
                var converted = latestMetadata.GetType().GetMethod("Convert")?.Invoke(latestMetadata, Array.Empty<object>());
                if (converted is null)
                    return null;
                latestMetadata = (converted as VersioningData)!;
            }
            return latestMetadata as SongChartData;
        }

        public static SongData? ConvertToLatestMetadata(VersioningData data)
        {
            if (data is not (SongData or Data.v21X.SongData))
                return null;
            
            var latestMetadata = data;
            while (latestMetadata.GetType().GetInterfaces().FirstOrDefault(i => i.Name.StartsWith("IVersionConvertible")) != null)
            {
                var converted = latestMetadata.GetType().GetMethod("Convert")?.Invoke(latestMetadata, Array.Empty<object>());
                if (converted is null)
                    return null;
                latestMetadata = (converted as VersioningData)!;
            }
            return latestMetadata as SongData;
        }

        public static ISongPack<SongData, SongChartData>? ConvertToLatest(ISongPack<VersioningData, VersioningData> data)
        {
            var metadata = data.Metadata;
            var chart = data.Chart;
            if (chart is null)
                return null;
            var newChart = ConvertToLatestChartData(chart);
            SongData? newMetadata = null;
            if (metadata is null && chart is Data.v10X.SongChartData legacyChart)
            {
                newMetadata = new SongData(legacyChart.Song.Song, "", "default")
                {
                    TimeFormat = "ms",
                    TimeChanges = new[]
                    {
                        new SongTimeChange(-1, legacyChart.Song.Bpm)
                    },
                    PlayData = new SongPlayData
                    {
                        Album = "",
                        Characters = new SongCharacterData
                        {
                            Player = legacyChart.Song.Player1,
                            Opponent = legacyChart.Song.Player2,
                            Girlfriend = legacyChart.Song.Player3
                        },
                        Difficulties = new[] { "normal" },
                        NoteStyle = "funkin",
                        Ratings = new()
                        {
                            { "normal", 1 }
                        },
                        SongVariations = new[] { "default" },
                        Stage = legacyChart.Song.Stage
                    },
                    GeneratedBy = $"FunkinParser conversion [1.0.0 -> {SongData.DefaultVersion.ToNormalizedString()}]"
                };
            }
            else if (metadata is not null)
            {
                newMetadata = ConvertToLatestMetadata(metadata);
            }
            return new SongPack<SongData, SongChartData>(newMetadata, newChart);
        }
    }
}