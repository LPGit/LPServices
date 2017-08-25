using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LPWpf.Behaviours
{
    public static class FocusBehaviour
    {



        public static bool GetFocusedFirstElementOnStartup(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusedFirstElementOnStartupProperty);
        }

        public static void SetFocusedFirstElementOnStartup(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusedFirstElementOnStartupProperty, value);
        }

        public static readonly DependencyProperty FocusedFirstElementOnStartupProperty =
            DependencyProperty.RegisterAttached("FocusedFirstElementOnStartup", typeof(bool), typeof(FocusBehaviour), new PropertyMetadata(false, OnFocusedFirstElementOnStartupPropertyChanged));


        static void OnFocusedFirstElementOnStartupPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement control = obj as FrameworkElement;
            if (control == null || !(args.NewValue is bool))
                return;


            if ((bool)args.NewValue)
                control.Loaded += (sender, e) => control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

        }

    }
}
