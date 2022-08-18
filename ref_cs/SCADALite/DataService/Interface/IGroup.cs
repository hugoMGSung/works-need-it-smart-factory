using System;
using System.Collections.Generic;

namespace DataService
{
    public interface IGroup : IDisposable
    {
        bool IsActive { get; set; }
        short ID { get; }
        int UpdateRate { get; set; }
        float DeadBand { get; set; }
        string Name { get; set; }
        IDriver Parent { get; }
        IDataServer Server { get; }
        IEnumerable<ITag> Items { get; }
        bool AddItems(IList<TagMetaData> items);
        bool AddTags(IEnumerable<ITag> tags);
        bool RemoveItems(params ITag[] items);
        bool SetActiveState(bool active, params short[] items);
        ITag FindItemByAddress(DeviceAddress addr);
        HistoryData[] BatchRead(DataSource source, bool isSync, params ITag[] itemArray);
        int BatchWrite(SortedDictionary<ITag, object> items, bool isSync = true);

        ItemData<int> ReadInt32(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<uint> ReadUInt32(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<short> ReadInt16(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<ushort> ReadUInt16(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<byte> ReadByte(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<float> ReadFloat(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<bool> ReadBool(DeviceAddress address, DataSource source = DataSource.Cache);
        ItemData<string> ReadString(DeviceAddress address, DataSource source = DataSource.Cache);

        int WriteInt32(DeviceAddress address, int value);
        int WriteUInt32(DeviceAddress address, uint value);
        int WriteInt16(DeviceAddress address, short value);
        int WriteUInt16(DeviceAddress address, ushort value);
        int WriteFloat(DeviceAddress address, float value);
        int WriteString(DeviceAddress address, string value);
        int WriteBit(DeviceAddress address, bool value);
        int WriteBits(DeviceAddress address, byte value);

        event DataChangeEventHandler DataChange;
    }
}
