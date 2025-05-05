using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funkin.Core.Data;
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
            AddMetadataType<Data.v20X.SongData>(VersionRange.Parse("[2.0.0,2.1.0)"));
            AddMetadataType<Data.v21X.SongData>(VersionRange.Parse("[2.1.0,2.2.0)"));
            AddMetadataType<Data.v22X.SongData>(VersionRange.Parse("[2.2.0,2.3.0)"));
            AddChartDataType<Data.v20X.SongChartData>(VersionRange.Parse("[2.0.0,2.3.0)"));
        }
        
        public FunkinParser(string metadataJsonText, string chartJsonText)
        {
            MetadataJsonText = metadataJsonText;
            ChartJsonText = chartJsonText;
        }
        
        public IData? ParseMetadata()
        {
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
            var partialChartData = JsonConvert.DeserializeObject<VersioningData>(ChartJsonText);
            if (partialChartData is null)
                throw new NullReferenceException("Failed to deserialize chart data!");
            
            var type = CompleteChartDataTypes.FirstOrDefault(t => t.Key.Satisfies(partialChartData.Version)).Value;
            if (type is null)
                throw new Exception($"Couldn't find chart data model for version '{partialChartData.Version}'.");
            
            var instanceOfChartData = JsonConvert.DeserializeObject(ChartJsonText, type);
            return instanceOfChartData as IData;
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
    }
}