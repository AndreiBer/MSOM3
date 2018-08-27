using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xvue.MSOT.Services.Log;

namespace ViewMSOT.UIControls
{
    public delegate bool? ModalWindowCallback(string caption,UserControl content, object dataContext, DependencyProperty controlProperty);

    public delegate void MessageLogCallback(EnumLogType logType, string messageHeader, string messageReason);

    public static class DllEntryPoint
    {
        public static ModalWindowCallback ModalWindow { get; set; }

        internal static bool? ShowModalWindow(string caption,UserControl content, object dataContext,DependencyProperty controlProperty)
        {
            if (ModalWindow != null)
            {
                return ModalWindow(caption, content, dataContext, controlProperty);
            }
            else return null;
        }

        public static MessageLogCallback MessageLog { get; set; }

        internal static void LogMessage(EnumLogType logType, string messageHeader, string messageReason)
        {
            if (MessageLog != null)
            {
                MessageLog(logType, messageHeader, messageReason);
            }
        }

    }
}
