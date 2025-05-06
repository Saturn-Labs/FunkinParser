using System;
using System.Collections.Generic;

namespace Funkin.Core
{
    public interface ISongPack<out TMeta, out TChart>
    {
        TMeta? Metadata { get; }
        TChart? Chart { get; }
    }
    
    public struct SongPack<TMeta, TChart> : ISongPack<TMeta, TChart>, IEquatable<SongPack<TMeta, TChart>>
    {
        public bool Equals(SongPack<TMeta, TChart> other)
        {
            return EqualityComparer<TMeta?>.Default.Equals(Metadata, other.Metadata) && EqualityComparer<TChart?>.Default.Equals(Chart, other.Chart);
        }

        public override bool Equals(object? obj)
        {
            return obj is SongPack<TMeta, TChart> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Metadata, Chart);
        }

        public TMeta? Metadata { get; }
        public TChart? Chart { get; }

        public SongPack(TMeta? metadata, TChart? chart)
        {
            Metadata = metadata;
            Chart = chart;
        }

        public bool HasMetadata => Metadata != null;
        public bool HasChart => Chart != null;
        public bool IsValid => HasMetadata || HasChart;

        public bool Equals(SongPack<TMeta, TChart>? other)
        {
            if (other is null) 
                return false;
            return Equals(Metadata, other.Value.Metadata) && Equals(Chart, other.Value.Chart);
        }
        
        public override string ToString()
        {
            return $"SongPack({Metadata}, {Chart})";
        }
        
        public static bool operator==(SongPack<TMeta, TChart>? left, SongPack<TMeta, TChart>? right) => left?.Equals(right) == true;
        public static bool operator!=(SongPack<TMeta, TChart>? left, SongPack<TMeta, TChart>? right) => left?.Equals(right) == false;
    }
}