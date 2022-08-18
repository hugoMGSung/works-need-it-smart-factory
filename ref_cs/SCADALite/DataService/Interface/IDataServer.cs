using System;
using System.Collections.Generic;

namespace DataService
{
    public interface IDataServer : IDisposable
    {
        ITag this[short id] { get; }
        ITag this[string name] { get; }
        ExpressionEval Eval { get; }
        Object SyncRoot { get; } // IGROUP의 ADDITEMS를 포함하여 컬렉션 변경과 관련된 모든 항목에 사용
        IList<TagMetaData> MetaDataList { get; }
        IList<Scaling> ScalingList { get; }
        IEnumerable<IDriver> Drivers { get; }
        IEnumerable<string> BrowseItems(BrowseType browseType, string tagName, DataType dataType);
        IDriver AddDriver(short id, string name, string assembly, string className);
        IGroup GetGroupByName(string name);
        int GetScaleByID(short id);
        int GetItemProperties(short id); // 반환되는 것은 메타데이터 목록에 있는 메타데이터의 인덱스
        bool RemoveDriver(IDriver device);
        bool AddItemIndex(string key, ITag value);
        bool RemoveItemIndex(string key);
        void ActiveItem(bool active, params ITag[] items);
        int BatchWrite(Dictionary<string, object> tags, bool sync);
    }
}
