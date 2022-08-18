using System;
using System.Collections.Generic;

namespace DataService
{
    public interface IDriver : IDisposable
    {
        short ID { get; }
        string Name { get; }
        bool IsClosed { get; }
        int TimeOut { get; set; }
        IEnumerable<IGroup> Groups { get; }
        IDataServer Parent { get; }
        bool Connect();
        IGroup AddGroup(string name, short id, int updateRate, float deadBand = 0f, bool active = false);
        bool RemoveGroup(IGroup group);
        event IOErrorEventHandler OnError;
    }

    public interface IPLCDriver : IDriver, IReaderWriter
    {
        int PDU { get; }
        DeviceAddress GetDeviceAddress(string address);
        string GetAddress(DeviceAddress address);
    }

    public interface IFileDriver : IDriver, IReaderWriter
    {
        string FileName { get; set; }
        FileData[] ReadAll(short groupId);
        //bool RecieveData(string data);
    }
}
