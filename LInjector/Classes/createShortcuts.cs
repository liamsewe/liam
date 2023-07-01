﻿using System;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace LInjector.Classes
{
    public static class CreateShortcuts 
    {
        static string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string RobloxACFolder = Path.Combine(localAppDataFolder, "Packages", "ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr", "AC");
        static string workspaceFolder = Path.Combine(RobloxACFolder, "workspace");
        static string autoexecFolder = Path.Combine(RobloxACFolder, "autoexec");
        static WshShell wsh = new WshShell();

        public static void Create ()
        {
            if (!System.IO.File.Exists(".\\workspace.lnk"))
            {
                IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(".\\workspace.lnk");
                shortcut.TargetPath = workspaceFolder;
                shortcut.Save();
            }

            if (!System.IO.File.Exists(".\\autoexec.lnk"))
            {
                IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(".\\autoexec.lnk");
                shortcut.TargetPath = autoexecFolder;
                shortcut.Save();
            }
        }
    }
}