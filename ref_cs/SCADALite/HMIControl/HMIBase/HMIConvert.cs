using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMIControl
{
    public class BoolToVisibleOrHidden : IValueConverter
    {
        #region Constructors

        public BoolToVisibleOrHidden() { }
        #endregion

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        #endregion
    }

    public class StoreCVToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 127;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2Visible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StoreCVToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value == 0.0) ? 0 : 0.25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_ScaleStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "피딩", "언로딩", "완료", "대기중", "테스트", "", "", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_MixerStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "모터시작", "건식혼합", "습식혼합", "언로딩", "언로딩", "완료", "", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_LiquidBufStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "", "대기중", "언로딩", "완료", "비정상", "분사", "", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_HandAddStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "추가", "대기중", "대기중", "언로딩", "완료", "오류", "", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_LiquidAddStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "피딩", "대기중", "대기중", "언로딩", "완료", "오류", "테스트", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_CheckStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "지연", "픽업", "대기중", "언로딩", "완료", "오류", "공급", "일시정지", "유지" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_BufferStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "", "대기중", "언로딩", "완료", "", "", "", "일시정지", "" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int16_GrindStatusConverter : IValueConverter
    {
        string[] list = new string[] { "유휴", "장치시작", "초기조정", "조정시작", "운행조정", "과부하", "자동종료", "비상정지", "일시정지", "" };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short index = (short)value;
            string ret = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (((1 << i) & index) != 0)
                    ret += list[i] + ".";
            }
            return ret.TrimEnd('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
