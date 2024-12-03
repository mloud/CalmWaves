using System.Collections.Generic;

namespace OneDay.Core.Modules.Data
{
    public interface ITable<T> where T: IDataObject
    {
        public List<T> Data { get; }
    }
}