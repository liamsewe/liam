﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

/*
 * 
 * ░▒█▀▀▀░█░░█░▒█░█░█░█░▒█░█▀▀░░░█▀▀▄░▒█▀▀█░▀█▀
 * ░▒█▀▀░░█░░█░▒█░▄▀▄░█░▒█░▀▀▄░░▒█▄▄█░▒█▄▄█░▒█░
 * ░▒█░░░░▀▀░░▀▀▀░▀░▀░░▀▀▀░▀▀▀░░▒█░▒█░▒█░░░░▄█▄
 *
 * Fluxus UWP API (https://fluxteam.net)
 * Provided by Fluxteam.
 */

namespace LInjector.Classes
{
    // Token: 0x02000006 RID: 6
    public static class FluxusAPI
    {
        public enum Result : uint
        {
            Success,
            DLLNotFound,
            OpenProcFail,
            AllocFail,
            LoadLibFail,
            AlreadyInjected,
            ProcNotOpen,
            Unknown
        }

        public static string dll_path;

        public static IntPtr phandle;

        public static int pid;

        private static readonly IntPtr NULL = (IntPtr)0;

        // Token: 0x06000013 RID: 19
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, bool inhert_handle, int pid);

        // Token: 0x06000014 RID: 20
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize,
            uint flAllocationType, uint flProtect);

        // Token: 0x06000015 RID: 21
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
            IntPtr nSize, int lpNumberOfBytesWritten);

        // Token: 0x06000016 RID: 22
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // Token: 0x06000017 RID: 23
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Token: 0x06000018 RID: 24
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize,
            IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // Token: 0x06000019 RID: 25
        [DllImport("Resources\\libs\\FluxteamAPI.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool run_script(IntPtr proc, int pid, string path,
            [MarshalAs(UnmanagedType.LPWStr)] string script);

        // Token: 0x0600001A RID: 26
        [DllImport("Resources\\libs\\FluxteamAPI.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool is_injected(IntPtr proc, int pid, string path);

        // Token: 0x0600001B RID: 27
        [DllImport("Resources\\libs\\FluxteamAPI.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool inject_dll(int pid, [MarshalAs(UnmanagedType.LPWStr)] string script);

        // Token: 0x0600001C RID: 28 RVA: 0x0000287C File Offset: 0x00000A7C
        private static Result r_inject(string dll_path)
        {
            var fileInfo = new FileInfo(dll_path);
            var accessControl = fileInfo.GetAccessControl();
            var identity = new SecurityIdentifier("S-1-15-2-1");
            accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl,
                InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            fileInfo.SetAccessControl(accessControl);
            var processesByName = Process.GetProcessesByName("Windows10Universal");
            var flag = processesByName.Length == 0;
            Result result;
            if (flag)
            {
                result = Result.ProcNotOpen;
            }
            else
            {
                var num = 0U;
                while (num < (ulong)processesByName.Length)
                {
                    var process = processesByName[(int)num];
                    var flag2 = pid != process.Id;
                    if (flag2)
                    {
                        var intPtr = OpenProcess(1082U, false, process.Id);
                        var flag3 = intPtr == NULL;
                        if (flag3)
                        {
                            return Result.OpenProcFail;
                        }
                        var intPtr2 = VirtualAllocEx(intPtr, NULL,
                            (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 12288U, 64U);
                        var flag4 = intPtr2 == NULL;
                        if (flag4)
                        {
                            return Result.AllocFail;
                        }
                        var bytes = Encoding.Default.GetBytes(dll_path);
                        var num2 = WriteProcessMemory(intPtr, intPtr2, bytes,
                            (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 0);
                        var flag5 = num2 == 0 || num2 == 6L;
                        if (flag5)
                        {
                            return Result.Unknown;
                        }
                        var flag6 = CreateRemoteThread(intPtr, NULL, NULL,
                            GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA"), intPtr2, 0U, NULL) == NULL;
                        if (flag6)
                        {
                            return Result.LoadLibFail;
                        }
                        pid = process.Id;
                        phandle = intPtr;
                        return Result.Success;
                    }

                    var flag7 = pid == process.Id;
                    if (flag7)
                    {
                        return Result.AlreadyInjected;
                    }
                    num += 1U;
                }

                result = Result.Unknown;
            }

            return result;
        }

        public static Result inject_custom()
        {
            Result result;
            try
            {
                var flag = !File.Exists(dll_path);
                if (flag)
                {
                    result = Result.DLLNotFound;
                }
                else
                {
                    result = r_inject(dll_path);
                    try
                    { application app = new application(); _ = NotificationManager.FireNotification($"Successfully attached to Roblox UWP with PID: {FluxusAPI.pid}", app.infSettings); }
                    catch { }
                }
            }
            catch
            {
                result = Result.Unknown;
            }

            return result;
        }

        public static void inject()
        {
            switch (inject_custom())
            {
                case Result.DLLNotFound:
                    ThreadBox.MsgThread("Dynamic-Link Library were not found\n",
                        "Injection Error | Fluxteam API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Result.OpenProcFail:
                    ThreadBox.MsgThread("OpenProcess Failed\n", "Injection Error | Fluxteam API",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Result.AllocFail:
                    ThreadBox.MsgThread("Allocation Failed\n", "Injection Error | Fluxteam API",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Result.LoadLibFail:
                    ThreadBox.MsgThread("LoadLibrary Failed\n", "Injection Error | Fluxteam API",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Result.ProcNotOpen:
                    ThreadBox.MsgThread(
                        "Failure to find Roblox UWP.\nMake sure you have Roblox from the Microsoft Store",
                        "Injection Error | Fluxteam API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Result.Unknown:
                    ThreadBox.MsgThread("Unknown Error\n", "Injection Error | Fluxteam API",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        public static bool is_injected(int pid)
        {
            try
            {
                phandle = OpenProcess(0x43AU, false, pid);
                return is_injected(phandle, pid, dll_path);
            }
            catch
            {
                return false;
            }
        }

        public static bool run_script(int pid, string script)
        {
            FluxusAPI.pid = pid;
            phandle = OpenProcess(1082U, false, pid);
            var flag = pid == 0;
            bool result;
            if (flag)
            {
                ThreadBox.MsgThread("Inject API First", "Fluxus | API Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            else
            {
                var flag2 = script == string.Empty;
                if (flag2)
                    result = is_injected(pid);
                else
                    result = run_script(phandle, pid, dll_path, script);
            }

            return result;
        }

        public static void create_files(string dll_path_)
        {
            var flag = !File.Exists(dll_path_);
            if (flag)
            {
                ThreadBox.MsgThread("Failure when initializing Fluxus API\nDLL path was invalid\n",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dll_path = dll_path_;
            var text = "";
            foreach (var text2 in Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + "\\Packages"))
            {
                var flag2 = text2.Contains("ROBLOXCORPORATION");
                if (flag2)
                {
                    var flag3 = Directory.GetDirectories(text2 + "\\AC").Any(dir => dir.Contains("Temp"));
                    if (flag3)
                    {
                        text = text2 + "\\AC";
                    }
                }
            }

            var flag4 = text == "";
            if (!flag4)
            {
                try
                {
                    var flag5 = Directory.Exists("workspace");
                    if (flag5)
                    {
                        Directory.Move("workspace", "old_workspace");
                    }
                    var flag6 = Directory.Exists("autoexec");
                    if (flag6)
                    {
                        Directory.Move("autoexec", "old_autoexec");
                    }
                }
                catch
                {
                }

                var path = Path.Combine(text, "workspace");
                var path2 = Path.Combine(text, "autoexec");
                var flag7 = !Directory.Exists(path);
                if (flag7)
                {
                    Directory.CreateDirectory(path);
                }
                var flag8 = !Directory.Exists(path2);
                if (flag8)
                {
                    Directory.CreateDirectory(path2);
                }
            }
        }
    }
}