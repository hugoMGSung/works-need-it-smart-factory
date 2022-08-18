using System;
using System.Collections.Generic;
using System.Windows;

namespace HMIControl
{
    public struct ConnectInfo
    {
        public Rect DesignerRect;

        public Point Position;

        public ConnectOrientation Orient;

        public static readonly ConnectInfo Empty;

        static ConnectInfo()
        {
            Empty = new ConnectInfo();
        }

        public static bool operator ==(ConnectInfo info1, ConnectInfo info2)
        {
            return ((info1.DesignerRect == info2.DesignerRect) &&
                    (info1.Position == info2.Position) &&
                    (info1.Orient == info2.Orient));
        }

        public static bool operator !=(ConnectInfo info1, ConnectInfo info2)
        {
            return !(info1 == info2);
        }

        public static bool Equals(ConnectInfo info1, ConnectInfo info2)
        {
            return info1 == info2;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ConnectInfo)) return false;
            return Equals(this, (ConnectInfo)obj);
        }

        public override int GetHashCode()
        {
            return this.DesignerRect.GetHashCode() ^ this.Position.GetHashCode() ^ this.Orient.GetHashCode();
        }
    }

    public enum ConnectOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    public interface ITagLink
    {
        string Node { get; }
    }

    public interface ITagReader : ITagLink
    {
        // like ==> fan.运行:Receiving1_Fan2_Running\fan.设备名:Receiving1_Fan2\fan.报警:Receiving1_Fan2_Alarm\
        string TagReadText { get; set; }  // TODO: 화면에서 각 모듈별 필요값 변경요
        string[] GetActions();
        Action SetTagReader(string key, Delegate tagChanged);
        IList<ITagLink> Children { get; }
    }

    public interface ITagWindow : ITagLink
    {
        bool IsModel { get; set; }
        bool IsUnique { get; set; }
        string TagWindowText { get; set; }
    }

    public interface ITagWriter : ITagLink
    {
        bool IsPulse { get; set; }
        string TagWriteText { get; set; }
        bool SetTagWriter(IEnumerable<Delegate> tagWriter);
    }

    public class TagActions
    {
        public const string RUN = "운행";
        public const string ALARM = "알람";
        public const string SP = "이론치";
        public const string PV = "실측치";
        public const string BYPASS = "우회";
        public const string RAWNAME = "재료명";
        public const string CAPTION = "캡션";
        public const string TEXT = "본문";
        public const string ON = "온";
        public const string OFF = "오프";
        public const string ONOFF = "온/오프";
        public const string START = "시동";
        public const string STOP = "중지";
        public const string DEVICENAME = "장치명";
        public const string ONALARM = "알람온";
        public const string OFFALARM = "알람오프";
        public const string LEFTALARM = "좌알람";
        public const string RIGHTALARM = "우알람";
        public const string WARN = "경고";
        public const string PRESS = "프레스";
        public const string HIGHLEVEL = "고레벨";
        public const string LOWLEVEL = "저레벨";
        public const string SPEED = "속도";
        public const string AMPS = "전류";
        public const string LEFT = "좌";
        public const string RIGHT = "우";
        public const string MID = "중";
        public const string STATE = "상태변경";
        public const string STATE1 = "상태1";
        public const string STATE2 = "상태2";
        public const string STATE3 = "상태3";
        public const string STATE4 = "상태4";
        public const string STATE5 = "상태5";
        public const string STATE6 = "상태6";
        public const string STATE7 = "상태7";
        public const string STATE8 = "상태8";
        public const string VISIBLE = "가시";
        public const string ENABLE = "가능";
        public const string DISABLE = "불가";
    }
}
