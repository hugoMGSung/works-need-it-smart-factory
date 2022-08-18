using System;

namespace DataService
{
    public interface IReaderWriter
    {
        byte[] ReadBytes(DeviceAddress address, ushort size);
        ItemData<uint> ReadUInt32(DeviceAddress address);
        ItemData<int> ReadInt32(DeviceAddress address);
        ItemData<ushort> ReadUInt16(DeviceAddress address);
        ItemData<short> ReadInt16(DeviceAddress address);
        ItemData<byte> ReadByte(DeviceAddress address);
        ItemData<string> ReadString(DeviceAddress address, ushort size);
        ItemData<float> ReadFloat(DeviceAddress address);
        ItemData<bool> ReadBit(DeviceAddress address);
        ItemData<object> ReadValue(DeviceAddress address);

        int WriteBytes(DeviceAddress address, byte[] bit);
        int WriteBit(DeviceAddress address, bool bit);
        int WriteBits(DeviceAddress address, byte bits);
        int WriteInt16(DeviceAddress address, short value);
        int WriteUInt16(DeviceAddress address, ushort value);
        int WriteInt32(DeviceAddress address, int value);
        int WriteUInt32(DeviceAddress address, uint value);
        int WriteFloat(DeviceAddress address, float value);
        int WriteString(DeviceAddress address, string str);
        int WriteValue(DeviceAddress address, object value);
    }

    public interface ICache : IReaderWriter
    {
        int Size { get; set; }
        int ByteCount { get; }
        Array Cache { get; }
        int GetOffset(DeviceAddress start, DeviceAddress end);
    }

    public interface IMultiReadWrite
    {
        int Limit { get; }
        ItemData<Storage>[] ReadMultiple(DeviceAddress[] addrsArr);
        int WriteMultiple(DeviceAddress[] addrArr, object[] buffer);
    }
}
