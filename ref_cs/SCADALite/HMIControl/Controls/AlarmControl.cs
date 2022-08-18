using System;
using System.Windows;
using System.ComponentModel;

namespace HMIControl
{
    /// <summary>
    /// UserControl2.xaml 코드비하인드
    /// </summary>
    public class AlarmControl : HMIControlBase
    {
        static AlarmControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlarmControl), new FrameworkPropertyMetadata(typeof(AlarmControl)));
        }

        public static readonly DependencyProperty Alarm1Property = DependencyProperty.Register("Alarm1", typeof(bool), typeof(AlarmControl),
            new PropertyMetadata(new PropertyChangedCallback(ValueChangedCallback)));


        #region HMI 속성

        [Category("HMI")]
        public bool Alarm1
        {
            set
            {
                SetValue(Alarm1Property, value);
            }
            get
            {
                return (bool)GetValue(Alarm1Property);
            }
        }

        #endregion

        private static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as AlarmControl).UpdataState(false);
        }

        public override string[] GetActions()
        {
            return new string[] { TagActions.VISIBLE, TagActions.CAPTION, TagActions.DEVICENAME, TagActions.ALARM };
        }

        public override Action SetTagReader(string key, Delegate tagChanged)
        {
            switch (key)
            {
                case TagActions.ALARM:
                    var _funcAlarm = tagChanged as Func<bool>;
                    if (_funcAlarm != null)
                    {
                        return delegate
                        {
                            if (_funcAlarm())
                                VisualStateManager.GoToState(this, "Alarm", true);
                            else
                                VisualStateManager.GoToState(this, "Normal", true);

                        };
                    }
                    else return null;
                
            }
            return base.SetTagReader(key, tagChanged);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdataState(false);
            
        }

        private void UpdataState(bool myState)
        {
            if (Alarm)
            { VisualStateManager.GoToState(this, "Alarm", true); }
            else
            { VisualStateManager.GoToState(this, "Normal", true); }
        }


    }
}

