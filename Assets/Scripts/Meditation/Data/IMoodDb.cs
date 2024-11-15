using System.Collections.Generic;

namespace Meditation.Data
{
    public interface IMoodDb
    {
        int MaxMoodsSelected { get; }
        IEnumerable<string> Moods { get; }
    }
}