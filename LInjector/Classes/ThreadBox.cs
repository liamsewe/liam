﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace LInjector.Classes
{
    public static class ThreadBox
    {
        public static void MsgThread(string msgBoxContent = "",
            string msgBoxTitle = "",
            MessageBoxButtons boxButtons = MessageBoxButtons.OK,
            MessageBoxIcon boxIcon = MessageBoxIcon.None,
            MessageBoxDefaultButton boxDefaultButton = MessageBoxDefaultButton.Button1,
            MessageBoxOptions boxOptions = MessageBoxOptions.DefaultDesktopOnly)
        {
            var msgBoxThread = new Thread(
                () =>
                {
                    var dialogResult = MessageBox.Show(msgBoxContent, msgBoxTitle, boxButtons, boxIcon,
                        boxDefaultButton, boxOptions);
                }
            );
            msgBoxThread.Start();
            CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, msgBoxTitle + ", " + msgBoxContent, "DEBUG");
        }
    }
}