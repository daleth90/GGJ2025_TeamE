using System.Collections.Generic;

namespace Physalia
{
    public interface IGameDataManager
    {
        T GetData<T>(int id);
        bool TryGetData<T>(int id, out T value);
        IReadOnlyList<T> GetDataList<T>();
        T GetSettingTable<T>() where T : class;
        void Clear();
    }
}
