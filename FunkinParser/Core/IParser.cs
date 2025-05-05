using System.Threading.Tasks;

namespace Funkin.Core
{
    public interface IParser
    {
        IData? ParseMetadata();
        IData? ParseChartData();
    }
}