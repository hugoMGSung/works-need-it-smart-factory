using System.Collections.Generic;
using System.Timers;

namespace DataService
{
    public sealed class NetBytePLCGroup : PLCGroup
    {
        public NetBytePLCGroup(short id, string name, int updateRate, bool active, IPLCDriver plcReader)
        {
            this._id = id;
            this._name = name;
            this._updateRate = updateRate;
            this._isActive = active;
            this._plcReader = plcReader;
            this._server = _plcReader.Parent;
            this._timer = new Timer();
            this._changedList = new List<int>();
            this._cacheReader = new NetByteCacheReader();
        }

        protected override void Poll()
        {
            if (_items == null || _items.Count == 0) return;
            byte[] cache = (byte[])_cacheReader.Cache;
            int offset = 0;
            foreach (PDUArea area in _rangeList)
            {
                byte[] rcvBytes = _plcReader.ReadBytes(area.Start, (ushort)area.Len); // PLC에서 데이터 읽기             
                if (rcvBytes == null)
                {
                    // _plcReader.Connect();
                    continue;
                }
                else
                {
                    int index = area.StartIndex; // 인덱스는 _items의 태그 메타데이터를 포인팅
                    int count = index + area.Count;
                    while (index < count)
                    {
                        DeviceAddress addr = _items[index].Address;
                        int iByte = addr.CacheIndex;
                        int iByte1 = iByte - offset;
                        if (addr.VarType == DataType.BOOL)
                        {
                            int tmp = rcvBytes[iByte1] ^ cache[iByte];
                            DeviceAddress next = addr;
                            if (tmp != 0)
                            {
                                while (addr.Start == next.Start)
                                {
                                    if ((tmp & (1 << next.Bit)) > 0) _changedList.Add(index);
                                    if (++index < count)
                                        next = _items[index].Address;
                                    else
                                        break;
                                }
                            }
                            else
                            {
                                while (addr.Start == next.Start && ++index < count)
                                {
                                    next = _items[index].Address;
                                }
                            }
                        }
                        else
                        {
                            ushort size = addr.DataSize;
                            for (int i = 0; i < size; i++)
                            {
                                if (iByte1 + i < rcvBytes.Length && rcvBytes[iByte1 + i] != cache[iByte + i])
                                {
                                    _changedList.Add(index);
                                    break;
                                }
                            }
                            index++;
                        }
                    }
                    for (int j = 0; j < rcvBytes.Length; j++)
                        cache[j + offset] = rcvBytes[j]; // PLC에서 읽은 데이터를 CacheReader에 쓰기
                }
                offset += rcvBytes.Length;
            }
        }
    }
}
