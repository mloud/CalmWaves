using System.Collections.Generic;

namespace Meditation.Ui.Chart
{
    public interface IChartData<TX, TY>
    {
        TY MaxValue { get; set; } 
        List<(TX xValue, TY yValue)> Values { get; set; }
    }
  
    public interface IChart<TX, TY>
    {
        string Name { get; set; }
        string Units { get; set; }
        void Select(TX column);
        void Set(IChartData<TX, TY> data);
    }
}