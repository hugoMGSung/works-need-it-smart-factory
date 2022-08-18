using System;

namespace DataService
{
    /// <summary>
    /// 데이터소스
    /// </summary>
    public enum DataSource
    {
        Cache = 1,
        Device = 2
    }

    /// <summary>
    /// 데이터타입 열거형
    /// </summary>
    public enum DataType : byte
    {
        NONE = 0,
        BOOL = 1,
        BYTE = 3,       // 1byte
        SHORT = 4,      // 2byte
        WORD = 5,       // C++ WORD(unsigned short) 2byte
        DWORD = 6,      // C++ DWORD(unsigned long) 4byte
        INT = 7,        // 4byte
        FLOAT = 8,      // 8byte
        SYS = 9,        // object
        STR = 11        // ?
    }

    /// <summary>
    /// 바이트순서
    /// </summary>
    [Flags]
    public enum ByteOrder : byte
    {
        None = 0,
        BigEndian = 1,      // PA-RISC, SPARC, PowerPC
        LittleEndian = 2,   // x86, x64, Itanium
        Network = 4,
        Host = 8
    }

    /// <summary>
    /// 심각성 구분자 열거형
    /// </summary>
    public enum Severity
    {
        Error = 7,
        High = 6,
        MediumHigh = 5,
        Medium = 4,
        MediumLow = 3,
        Low = 2,
        Information = 1,
        Normal = 0
    }

    /// <summary>
    /// 품질 구분자 열거형
    /// </summary>
    public enum QUALITIES : short
    {
        QUALITY_BAD = 0,
        LIMIT_LOW = 1,
        LIMIT_HIGH = 2,
        LIMIT_CONST = 3,
        QUALITY_COMM_FAILURE = 0x18,    //24
        QUALITY_CONFIG_ERROR = 4,
        QUALITY_DEVICE_FAILURE = 12,
        QUALITY_EGU_EXCEEDED = 0x54,    // 84
        QUALITY_GOOD = 0xc0,
        QUALITY_LAST_KNOWN = 20,
        QUALITY_LAST_USABLE = 0x44,     // 68
        QUALITY_LOCAL_OVERRIDE = 0xd8,  // 216
        QUALITY_MASK = 0xc0,
        QUALITY_NOT_CONNECTED = 8,
        QUALITY_OUT_OF_SERVICE = 0x1c,  // 28
        QUALITY_SENSOR_CAL = 80,
        QUALITY_SENSOR_FAILURE = 0x10,  // 16
        QUALITY_SUB_NORMAL = 0x58,      // 88
        QUALITY_UNCERTAIN = 0x40,       // 64
        QUALITY_WAITING_FOR_INITIAL_DATA = 0x20,    // 32
        STATUS_MASK = 0xfc,             // 252
    }

    public enum BrowseType
    {
        Branch = 1,
        Leaf = 2,
        Flat = 3
    }


    public enum ScaleType : byte
    {
        None = 0,
        Linear = 1,
        SquareRoot = 2
    }
}