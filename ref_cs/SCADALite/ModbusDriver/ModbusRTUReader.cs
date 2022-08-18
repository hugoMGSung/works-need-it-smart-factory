using DataService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ModbusDriver
{
    [Description("Modbus RTU프로토콜")]
    public sealed class ModbusRTUReader : IPLCDriver
    {
        #region :IDriver
        // Slave 주소
        short _id;
        public short ID
        {
            get
            {
                return _id;
            }
        }
        // Doing: SlaveID를 슬레이브 주소로 추가
        public short SlaveID { set; get; }
        public string Name { set; get; }
        [Category("직렬포트설정"), Description("포트번호")]
        public string PortName { set; get; }

        public bool IsClosed
        {
            get
            {
                return _serialPort == null || _serialPort.IsOpen == false;
            }
        }

        [Category("직렬포트설정"), Description("통신시간초과")]
        public int TimeOut { set; get; }

        [Category("직렬포트설정"), Description("전송속도")]
        public int BaudRate { set; get; }
        //   private SerialPort _serialPort;

        [Category("직렬포트설정"), Description("데이터비트")]
        public int DataBits { set; get; }

        private StopBits _stopBits = StopBits.One;
        [Category("직렬포트설정"), Description("정지비트")]
        public StopBits StopBits { set; get; }

        [Category("직렬포트설정"), Description("패리티체크")]
        public Parity Parity { set; get; }

        List<IGroup> _grps = new List<IGroup>();
        public IEnumerable<IGroup> Groups
        {
            get { return _grps; }
        }

        IDataServer _server;
        public IDataServer Parent
        {
            get { return _server; }
        }

        public bool Connect()
        {
            try
            {
                if (TimeOut <= 0) TimeOut = 1000;
                if (_serialPort == null)
                    _serialPort = new SerialPort(PortName);
                _serialPort.ReadTimeout = TimeOut;
                _serialPort.WriteTimeout = TimeOut;
                _serialPort.BaudRate = BaudRate;
                _serialPort.DataBits = DataBits;
                _serialPort.Parity = Parity;
                _serialPort.StopBits = _stopBits;
                _serialPort.Open();
                return true;
            }
            catch (IOException error)
            {
                if (OnError != null)
                {
                    OnError(this, new IOErrorEventArgs(error.Message));
                }
                return false;
            }
        }

        public IGroup AddGroup(string name, short id, int updateRate, float deadBand = 0f, bool active = false)
        {
            ShortGroup grp = new ShortGroup(id, name, updateRate, active, this);
            _grps.Add(grp);
            return grp;
        }

        public bool RemoveGroup(IGroup grp)
        {
            grp.IsActive = false;
            return _grps.Remove(grp);
        }
        public event IOErrorEventHandler OnError;
        #endregion
        // 사용자 정의 생성자 3
        public ModbusRTUReader(IDataServer server, short id, string name)
        {
            _id = id;
            Name = name;
            _server = server;
        }

        private SerialPort _serialPort;

        /*
            Sbyte :부호 있는 8비트 정수/ 값 범위 -128 ~ 127
　      　  Byte  :부호 없는 8비트 정수/ 값 범위 0 ~ 255
　     　   Short :부호 있는 16비트 정수/ 값 범위 -32768 ~ 32767
　          ushort:부호 있는 16비트 정수/ 값 범위 0 ~ 65,535
            Int   :부호 있는 32비트 정수/ 값 범위 -2147483648 ~ 2147483648
　　        uint  :부호 없는 32비트 정수/ 값 범위 0 ~ 4294967295
　     　   Long  :부호 있는 64비트 정수/ 값 범위 -9223372036854775808 ~ 9223372036854775808
　     　   Ulong :부호 없는 64비트 정수/ 값 범위 0 ~ 18446744073709551615。
        */
        private byte[] CreateReadHeader(int id, int startAddress, ushort length, byte function)
        {
            byte[] data = new byte[8];
            data[0] = (byte)id;				    // Slave id high byte   슬레이브주소
            data[1] = function;					// Message size
            byte[] _adr = BitConverter.GetBytes((short)startAddress); //지정된 16비트 부호 없는 정수 값을 바이트 배열 형식으로 반환
            //apply on small endian, TODO:support big endian
            data[2] = _adr[1];				// Start address 시작주소의 상위 8비트
            data[3] = _adr[0];				// Start address 시작주소의 하위 8비트
            byte[] _length = BitConverter.GetBytes((short)length);
            //apply on small endian, TODO:support big endian
            data[4] = _length[1];			// Number of data to read 레지스터 수의 상위 8비트
            data[5] = _length[0];			// Number of data to read 레지스터 수의 하위 8비트
            byte[] arr = Utility.CalculateCrc(data, 6);
            data[6] = arr[0]; // CRC 검사의 하위 8비트
            data[7] = arr[1]; // CRC 검사의 상위 8비트
            return data;
        }

        #region 단일 코일 또는 단일 개별 출력 쓰기 기능 코드: 0x05
        public int WriteSingleCoils(int id, int startAddress, bool OnOff)
        {
            byte[] data = new byte[8];
            data[0] = (byte)id;				// Slave id high byte
            data[1] = Modbus.fctWriteSingleCoil;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)startAddress);
            data[2] = _adr[1];				// Start address
            data[3] = _adr[0];              // Start address
            if (OnOff) data[4] = 0xFF;
            byte[] arr = Utility.CalculateCrc(data, 6);
            data[6] = arr[0];
            data[7] = arr[1];
            lock (_async)
            {
                _serialPort.Write(data, 0, data.Length);
                int numBytesRead = 0;
                var frameBytes = new byte[5];
                while (numBytesRead != frameBytes.Length)
                    numBytesRead += _serialPort.Read(frameBytes, numBytesRead, frameBytes.Length - numBytesRead);
                var slave = frameBytes[0];
                var code = frameBytes[1];
                if (code == 0x85) // 오류는 5바이트만 읽음
                {
                    var errorcode = frameBytes[2];
                    if (OnError != null)
                    {
                        OnError(this, new IOErrorEventArgs(Modbus.GetErrorString(errorcode)));
                    }
                    Thread.Sleep(10);
                    return -1;
                }
                else // 정확하려면 8바이트가 필요
                {
                    numBytesRead = 0;
                    while (numBytesRead < 3)
                        numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 3 - numBytesRead);
                    Thread.Sleep(10);
                    return 0;
                }
            }
        }
        #endregion

        #region 다중 코일 쓰기 기능 코드: 0x0F 15
        public int WriteMultipleCoils(int id, int startAddress, ushort numBits, byte[] values)
        {
            int len = values.Length;
            byte[] data = new byte[len + 9];
            data[0] = (byte)id;				// Slave id high byte  슬레이브 주소의 상위 8비트
            data[1] = Modbus.fctWriteMultipleCoils;				// Function code  기능 코드
            byte[] _adr = BitConverter.GetBytes((short)startAddress);
            data[2] = _adr[1];				// Start address       시작 주소 상위 8비트
            data[3] = _adr[0];				// Start address       시작 주소 하위 8비트
            byte[] _length = BitConverter.GetBytes((short)numBits);
            data[4] = _length[1];			// Number of data to read  레지스터 상위 8비트
            data[5] = _length[0];           // Number of data to read  레지스터 하위 8비트
            data[6] = (byte)len;            //바이트 수
            Array.Copy(values, 0, data, 7, len);  //데이터에 변경값추가
            byte[] arr = Utility.CalculateCrc(data, len + 7);
            data[len + 7] = arr[0];  //CRC 검사 하위 8비트
            data[len + 8] = arr[1];  //CRC 검사 상위 8비트
            lock (_async)
            {
                _serialPort.Write(data, 0, data.Length);
                int numBytesRead = 0;
                var frameBytes = new byte[5];
                while (numBytesRead != frameBytes.Length)
                    numBytesRead += _serialPort.Read(frameBytes, numBytesRead, frameBytes.Length - numBytesRead);
                var slave = frameBytes[0];
                var code = frameBytes[1];
                if (code == 0x85)//오류는 5바이트만 읽음
                {
                    var errorcode = frameBytes[2];
                    if (OnError != null)
                    {
                        OnError(this, new IOErrorEventArgs(Modbus.GetErrorString(errorcode)));
                    }
                    Thread.Sleep(10);
                    return -1;
                }
                else // 9바이트가 정확해야
                {
                    numBytesRead = 0;
                    while (numBytesRead < 4)
                        numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 3 - numBytesRead);
                    Thread.Sleep(10);
                    return 0;
                }
            }
        }
        #endregion

        #region 단일 홀딩 레지스터 쓰기 기능 코드:0x06
        public int WriteSingleRegister(int id, int startAddress, byte[] values)
        {
            byte[] data = new byte[8];
            data[0] = (byte)id;				// Slave id high byte 슬레이브 주소의 상위 8비트
            data[1] = Modbus.fctWriteSingleRegister;				// Function code 기능 코드
            byte[] _adr = BitConverter.GetBytes((short)startAddress);
            data[2] = _adr[1];				// Start address    시작 주소 상위 8비트
            data[3] = _adr[0];				// Start address    시작 주소 상위 8비트
            data[4] = values[1];            // 데이터 상위 비트 변경
            data[5] = values[0];            // 데이터 하위 비트 변경
            byte[] arr = Utility.CalculateCrc(data, 6);
            data[6] = arr[0];               //CRC 검사 코드 하위 8비트
            data[7] = arr[1];               //CRC 검사 코드 상위 8비트
            lock (_async)
            {
                _serialPort.Write(data, 0, data.Length);
                int numBytesRead = 0;
                var frameBytes = new byte[5];
                while (numBytesRead != frameBytes.Length)
                    numBytesRead += _serialPort.Read(frameBytes, numBytesRead, frameBytes.Length - numBytesRead);
                var slave = frameBytes[0];
                var code = frameBytes[1];
                if (code == 0x85)//오류는 5바이트만 읽음
                {
                    var errorcode = frameBytes[2];
                    if (OnError != null)
                    {
                        OnError(this, new IOErrorEventArgs(Modbus.GetErrorString(errorcode)));
                    }
                    Thread.Sleep(10);
                    return -1;
                }
                else//정확하려면 8바이트가 필요
                {
                    numBytesRead = 0;
                    while (numBytesRead < 3)
                        numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 3 - numBytesRead);
                    Thread.Sleep(10);
                    return 0;
                }
            }
        }
        #endregion

        #region 여러 홀딩 레지스터 쓰기 기능 코드：0x10    16
        public int WriteMultipleRegister(int id, int startAddress, byte[] values)
        {
            int len = values.Length;
            if (len % 2 > 0) len++;
            byte[] data = new byte[len + 9];
            data[0] = (byte)id;			// Slave id high byte  슬레이브 주소
            data[1] = Modbus.fctWriteMultipleRegister;				// Function code  기능 코드
            byte[] _adr = BitConverter.GetBytes((short)startAddress);
            data[2] = _adr[1];				// Start address        시작 주소 상위 8비트
            data[3] = _adr[0];				// Start address        시작 주소 하위 8비트
            byte[] _length = BitConverter.GetBytes((short)(len >> 1));
            data[4] = _length[1];			// Number of data to read 레지스터 상위 8비트
            data[5] = _length[0];			// Number of data to read 레지스터 하위 8비트
            data[6] = (byte)len;            //바이트 수
            Array.Copy(values, 0, data, 7, len); // 변경된 데이터를 값에 추가
            byte[] arr = Utility.CalculateCrc(data, len + 7);
            data[len + 7] = arr[0];          //CRC 검사 하위 8비트
            data[len + 8] = arr[1];          //CRC 검사 상위 8비트
            lock (_async)
            {
                _serialPort.Write(data, 0, data.Length);
                int numBytesRead = 0;
                var frameBytes = new byte[5];
                while (numBytesRead != frameBytes.Length)
                    numBytesRead += _serialPort.Read(frameBytes, numBytesRead, frameBytes.Length - numBytesRead);
                var slave = frameBytes[0];
                var code = frameBytes[1];
                if (code == 0x85)//오류는 5바이트만 읽음
                {
                    var errorcode = frameBytes[2];
                    if (OnError != null)
                    {
                        OnError(this, new IOErrorEventArgs(Modbus.GetErrorString(errorcode)));
                    }
                    Thread.Sleep(10);
                    return -1;
                }
                else//정확하려면 8바이트가 필요
                {
                    numBytesRead = 0;
                    while (numBytesRead < 3)
                        numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 3 - numBytesRead);
                    Thread.Sleep(10);
                    return 0;
                }
            }
        }
        #endregion

        #region  :IPLCDriver
        public int PDU
        {
            // get { return 0x100; } // 0x100 십진수 256
            /* 수정인：yjz
               수정일자：20171125
               업데이트 이유: RS232 / RS485 modbus 프로토콜은 직렬 통신에서 다음과 같이 규정
                         ADU = 주소 필드 + 기능 코드 + 데이터 + 오류 검사, 여기서 ADU는 256바이트, 
                               주소 필드는 1바이트, 기능코드는 1바이트, 데이터는 252바이트, 오류 검사는 2바이트.
                         PDU=기능코드+데이터
                         따라서 PDU는 253바이트여야 함 */
            get { return 0xFD; } // 0xFD 십진수 253
        }

        public DeviceAddress GetDeviceAddress(string address)
        {
            DeviceAddress dv = DeviceAddress.Empty;
            if (string.IsNullOrEmpty(address))
                return dv;
            var sindex = address.IndexOf(':');
            dv.Area = this.SlaveID;
            switch (address[0])
            {
                case '0':
                    {
                        dv.DBNumber = Modbus.fctReadCoil;
                        int st;
                        int.TryParse(address, out st);
                        dv.Bit = (byte)(st % 16);
                        st /= 16;
                        dv.Start = st;
                        dv.Bit--;
                    }
                    break;
                case '1':
                    {
                        dv.DBNumber = Modbus.fctReadDiscreteInputs;
                        int st;
                        int.TryParse(address.Substring(1), out st);
                        dv.Bit = (byte)(st % 16);
                        st /= 16;
                        dv.Start = st;
                        dv.Bit--;
                    }
                    break;
                case '4':
                    {
                        int index = address.IndexOf('.');
                        dv.DBNumber = Modbus.fctReadHoldingRegister;
                        if (index > 0)
                        {
                            dv.Start = int.Parse(address.Substring(1, index - 1));
                            dv.Bit = byte.Parse(address.Substring(index + 1));
                        }
                        else
                            dv.Start = int.Parse(address.Substring(1));
                        dv.Start--;
                        dv.Bit--;
                        dv.ByteOrder = ByteOrder.BigEndian;
                    }
                    break;
                case '3':
                    {
                        int index = address.IndexOf('.');
                        dv.DBNumber = Modbus.fctReadInputRegister;
                        if (index > 0)
                        {
                            dv.Start = int.Parse(address.Substring(1, index - 1));
                            dv.Bit = byte.Parse(address.Substring(index + 1));
                        }
                        else
                            dv.Start = int.Parse(address.Substring(1));
                        dv.Start--;
                        dv.Bit--;
                        dv.ByteOrder = ByteOrder.BigEndian;
                    }
                    break;
            }
            return dv;
        }

        public string GetAddress(DeviceAddress address)
        {
            return string.Empty;
        }
        #endregion

        #region :IReaderWriter 
        object _async = new object();
        public byte[] ReadBytes(DeviceAddress address, ushort size)
        {
            var func = (byte)address.DBNumber;
            try
            {
                byte[] header = func < 3 ? CreateReadHeader(address.Area, address.Start * 16, (ushort)(16 * size), func) :
                 CreateReadHeader(address.Area, address.Start, size, func);
                lock (_async)
                {
                    byte[] frameBytes = new byte[size * 2 + 5];//size * 2 +
                    byte[] data = new byte[size * 2];
                    _serialPort.Write(header, 0, header.Length);
                    int numBytesRead = 0;
                    while (numBytesRead < 2)
                        numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 2 - numBytesRead);
                    if (frameBytes[1] == address.DBNumber)
                    {
                        while (numBytesRead < frameBytes.Length)
                            numBytesRead += _serialPort.Read(frameBytes, numBytesRead, frameBytes.Length - numBytesRead);
                        if (Utility.CheckSumCRC(frameBytes))
                        {
                            Array.Copy(frameBytes, 3, data, 0, data.Length);
                            Thread.Sleep(20);
                            return data;
                        }
                        else Thread.Sleep(10);
                    }
                    else
                    {
                        numBytesRead = 0;
                        while (numBytesRead < 3)
                            numBytesRead += _serialPort.Read(frameBytes, numBytesRead, 3 - numBytesRead);
                        Thread.Sleep(10);
                    }

                }
                return null;
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError(this, new IOErrorEventArgs(e.Message));
                return null;
            }
        }

        public ItemData<int> ReadInt32(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 2);
            return bit == null ? new ItemData<int>(0, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<int>(BitConverter.ToInt32(bit, 0), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<uint> ReadUInt32(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 2);
            return bit == null ? new ItemData<uint>(0, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<uint>(BitConverter.ToUInt32(bit, 0), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<ushort> ReadUInt16(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 1);
            return bit == null ? new ItemData<ushort>(0, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<ushort>(BitConverter.ToUInt16(bit, 0), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<short> ReadInt16(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 1);
            return bit == null ? new ItemData<short>(0, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<short>(BitConverter.ToInt16(bit, 0), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<byte> ReadByte(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 1);
            return bit == null ? new ItemData<byte>(0, 0, QUALITIES.QUALITY_BAD) :
                 new ItemData<byte>(bit[0], 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<string> ReadString(DeviceAddress address, ushort size)
        {
            byte[] bit = ReadBytes(address, size);
            return bit == null ? new ItemData<string>(string.Empty, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<string>(Encoding.ASCII.GetString(bit), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<float> ReadFloat(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 2);
            return bit == null ? new ItemData<float>(0f, 0, QUALITIES.QUALITY_BAD) :
                new ItemData<float>(BitConverter.ToSingle(bit, 0), 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<bool> ReadBit(DeviceAddress address)
        {
            byte[] bit = ReadBytes(address, 1);
            return bit == null ? new ItemData<bool>(false, 0, QUALITIES.QUALITY_BAD) :
                 new ItemData<bool>((bit[0] & (1 << (address.Bit))) > 0, 0, QUALITIES.QUALITY_GOOD);
        }

        public ItemData<object> ReadValue(DeviceAddress address)
        {
            return this.ReadValueEx(address);
        }

        public int WriteBytes(DeviceAddress address, byte[] bit)
        {
            return WriteMultipleRegister(address.Area, address.Start, bit);
        }

        public int WriteBit(DeviceAddress address, bool bit)
        {
            return WriteSingleCoils(address.Area, address.Start + address.Bit, bit);
        }

        public int WriteBits(DeviceAddress address, byte bits)
        {
            return WriteSingleRegister(address.Area, address.Start, new byte[] { bits });
        }

        public int WriteInt16(DeviceAddress address, short value)
        {
            return WriteSingleRegister(address.Area, address.Start, BitConverter.GetBytes(value));
        }

        public int WriteUInt16(DeviceAddress address, ushort value)
        {
            return WriteSingleRegister(address.Area, address.Start, BitConverter.GetBytes(value));
        }

        public int WriteUInt32(DeviceAddress address, uint value)
        {
            return WriteMultipleRegister(address.Area, address.Start, BitConverter.GetBytes(value));
        }

        public int WriteInt32(DeviceAddress address, int value)
        {
            return WriteMultipleRegister(address.Area, address.Start, BitConverter.GetBytes(value));
        }

        public int WriteFloat(DeviceAddress address, float value)
        {
            return WriteMultipleRegister(address.Area, address.Start, BitConverter.GetBytes(value));
        }

        public int WriteString(DeviceAddress address, string str)
        {
            return WriteMultipleRegister(address.Area, address.Start, Encoding.ASCII.GetBytes(str));
        }

        public int WriteValue(DeviceAddress address, object value)
        {
            return this.WriteValueEx(address, value);
        }

        #endregion

        #region : IDisposable

        public void Dispose()
        {
            foreach (IGroup grp in _grps)
            {
                grp.Dispose();
            }
            _grps.Clear();
            _serialPort.Close();
        }
        #endregion
    }

    public sealed class Modbus
    {
        public const byte fctReadCoil = 1;
        public const byte fctReadDiscreteInputs = 2;
        public const byte fctReadHoldingRegister = 3;
        public const byte fctReadInputRegister = 4;
        public const byte fctWriteSingleCoil = 5;
        public const byte fctWriteSingleRegister = 6;
        public const byte fctWriteMultipleCoils = 15;
        public const byte fctWriteMultipleRegister = 16;
        public const byte fctReadWriteMultipleRegister = 23;

        /// <summary>예외가 잘못된 기능에 대한 상수</summary>
        public const byte excIllegalFunction = 1;
        /// <summary>예외 잘못된 데이터 주소에 대한 상수.</summary>
        public const byte excIllegalDataAdr = 2;
        /// <summary>예외 잘못된 데이터 주소에 대한 상수.</summary>
        public const byte excIllegalDataVal = 3;
        /// <summary>예외 슬레이브 장치 오류에 대한 상수.</summary>
        public const byte excSlaveDeviceFailure = 4;
        /// <summary>예외 승인에 대한 상수.</summary>
        public const byte excAck = 5;
        /// <summary>예외 슬레이브가 사용 중/부팅 중일 때 상수.</summary>
        public const byte excSlaveIsBusy = 6;
        /// <summary>사용할 수 없는 예외 게이트 경로에 대한 상수.</summary>
        public const byte excGatePathUnavailable = 10;
        /// <summary>연결되지 않은 예외에 대한 상수.</summary>
        public const byte excExceptionNotConnected = 253;
        /// <summary>예외 연결 손실에 대한 상수.</summary>
        public const byte excExceptionConnectionLost = 254;
        /// <summary>예외 응답 시간 초과에 대한 상수.</summary>
        public const byte excExceptionTimeout = 255;
        /// <summary>예외 잘못된 오프셋에 대한 상수.</summary>
        public const byte excExceptionOffset = 128;
        /// <summary>예외 전송 오류에 대한 상수.</summary>
        public const byte excSendFailt = 100;

        public static string GetErrorString(byte exception)
        {
            switch (exception)
            {
                case Modbus.excIllegalFunction:
                    return "ModbusModbus.exception 잘못된 기능에 대한 상수.";
                case Modbus.excIllegalDataAdr:
                    return "ModbusModbus.exception 잘못된 데이터 주소에 대한 상수.";
                case Modbus.excIllegalDataVal:
                    return "ModbusModbus.exception 잘못된 데이터 값에 대한 상수.";
                case Modbus.excSlaveDeviceFailure:
                    return "ModbusModbus.exception 슬레이브 장치 오류에 대한 상수.";
                case Modbus.excAck:
                    return "ModbusModbus.exception 승인에 대한 상수.";
                case Modbus.excSlaveIsBusy:
                    return "ModbusModbus.exception 슬레이브가 사용 중/부팅 중에 대한 상수.";
                case Modbus.excGatePathUnavailable:
                    return "ModbusModbus.exception 사용할 수 없는 게이트 경로에 대한 상수.";
                case Modbus.excExceptionNotConnected:
                    return "연결되지 않은 ModbusModbus.exception에 대한 상수.";
                case Modbus.excExceptionConnectionLost:
                    return "ModbusModbus.exception 연결 끊김에 대한 상수.";
                case Modbus.excExceptionTimeout:
                    return "ModbusModbus.exception 응답 시간 초과에 대한 상수.";
                case Modbus.excExceptionOffset:
                    return "ModbusModbus.exception의 잘못된 오프셋에 대한 상수.";
                case Modbus.excSendFailt:
                    return "Modbus Modbus.exception 전송 오류에 대한 상수.";
            }
            return string.Empty;
        }
    }
}
