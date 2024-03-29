﻿using System;
using System.IO;
using System.Media;
using System.Net;

namespace LInjector.Classes
{
    public static class DoPipe
    {
        private static readonly string metalPipeURL =
            "https://lexploits.netlify.app/extra/cdn/20826a3cb51d6c7d9c219c7f4bf4e5c9.wav";

        private static readonly string bambooPipeURL =
            "https://lexploits.netlify.app/extra/cdn/82960335036ff4ddc124b78af7777ee4.wav";

        private static readonly string tempDirectory = Path.GetTempPath();
        private static readonly string fileNameMetal = Path.GetFileName(metalPipeURL);
        private static readonly string fileNameBamboo = Path.GetFileName(bambooPipeURL);
        private static readonly string targetDirectory = Path.Combine(tempDirectory, "LInjector");
        public static string selectedArg;

        // METAL PIPE

        public static void doMetalPipeAsync()
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var filePath = Path.Combine(targetDirectory, fileNameMetal);

            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, "File downloaded successfully.", "DEBUG");
                        selectedArg = filePath;
                    }
                    else
                    {
                        CustomCw.Cw(ConsoleColor.Red, ConsoleColor.DarkRed, $"Error downloading file: {e.Error.Message}", "ERROR");
                    }
                };

                client.DownloadFileAsync(new Uri(metalPipeURL), filePath);
            }
        }

        // BAMBOO PIPE

        public static void doBambooPipeAsync()
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var filePath = Path.Combine(targetDirectory, fileNameBamboo);

            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, "File downloaded successfully.", "DEBUG");
                        selectedArg = filePath;
                    }
                    else
                    {
                        CustomCw.Cw(ConsoleColor.Red, ConsoleColor.DarkRed, $"Error downloading file: {e.Error.Message}", "ERROR");
                    }
                };

                client.DownloadFileAsync(new Uri(bambooPipeURL), filePath);
            }
        }

        public static void PlayPipeSound(string filePath)
        {
            CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, filePath);

            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                try
                {
                    using (var player = new SoundPlayer(filePath))
                    {
                        CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, "Playing pipe.", "DEBUG");
                        player.Play();
                    }
                }
                catch (Exception e)
                {
                    CustomCw.Cw(ConsoleColor.DarkRed, ConsoleColor.Red, $"Error playing file: {e.Message}", "ERROR");
                }
            }
            else
            {
                CustomCw.Cw(ConsoleColor.DarkGray, ConsoleColor.DarkGray, filePath, "DEBUG");
            }
        }
    }
}
