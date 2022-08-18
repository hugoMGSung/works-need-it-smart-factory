namespace DataService
{
    // Teltonika Network용 
    public class FCTCOMMAND
    {
        // 헤더는 암호화될 수 있으며, 일치하지 않으면 아무 작업도 안함
        // 클라이언트 소켓은 서버에 캡슐화된 알람 요청 전송
        public const byte fctHead = 0xAB;
        public const byte fctHdaIdRequest = 30;
        public const byte fctHdaRequest = 31;
        public const byte fctAlarmRequest = 32;
        public const byte fctOrderChange = 33;
        public const byte fctReset = 34;
        public const byte fctXMLHead = 0xEE; // XML 프로토콜
        public const byte fctReadSingle = 1;
        public const byte fctReadMultiple = 2;
        public const byte fctWriteSingle = 5;
        public const byte fctWriteMultiple = 15;
    }
}
