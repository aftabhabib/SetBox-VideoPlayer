﻿using Android.Util;
using Android.Widget;
using Xamarin.Forms;
using System;
using System.IO;
using System.Threading.Tasks;
using Rollbar;
using VideoPlayerProima.Helpers;
using ILogger = VideoPlayerProima.Interface.ILogger;

[assembly: Dependency(typeof(VideoPlayerProima.Droid.Controls.LoggerService))]
namespace VideoPlayerProima.Droid.Controls
{
    public class LoggerService : ILogger
    {

        private static readonly object lockSync = new object();

        private static LoggerService _instance;
        public static LoggerService Instance => _instance ?? (_instance = new LoggerService());

        public void Debug(string text)
        {
            Log.Debug("VideoPlayerProima", $"{text}");
            RollbarLocator.RollbarInstance.Log(ErrorLevel.Debug, text);
            SaveFile("DEBUG ", text, null);
        }

        public void Error(string text, Exception ex)
        {
            Log.Error("VideoPlayerProima", $"{text} - {ex.Message}");
            RollbarLocator.RollbarInstance.Log(ErrorLevel.Error, ex);
            SaveFile("ERRO  ", text, ex);
            Toast.MakeText(MainActivity.Instance, $"{text} - {ex.Message}", ToastLength.Long).Show();
        }

        public void Error(Exception ex)
        {
            Log.Error("VideoPlayerProima", $"{ex.Message}");
            RollbarLocator.RollbarInstance.Log(ErrorLevel.Error, ex);
            SaveFile("ERRO  ", null, ex);
            Toast.MakeText(MainActivity.Instance, $"{ex.Message}", ToastLength.Long).Show();
        }

        public void Info(string text)
        {
            Log.Info("VideoPlayerProima", $"{text}");
            RollbarLocator.RollbarInstance.Log(ErrorLevel.Info, text);
            SaveFile("INFO  ", text, null);
        }


        private void SaveFile(string tipo,  string text, Exception ex)
        {
            Task.Run(() =>
            {
                try
                {
                    lock (lockSync)
                    {
                        //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"LOG-{DateTime.Now:yyyyddmm}.txt");
                        string directory = Path.Combine(PlayerSettings.PathFiles, "LOGS");

                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        string fileName = Path.Combine(directory, $"LOG-{DateTime.Now:yyyy-MM-dd}.txt");


                        using (var streamWriter = !File.Exists(fileName)
                            ? File.CreateText(fileName)
                            : new StreamWriter(fileName, true))
                        {
                            if (!string.IsNullOrEmpty(text))
                                streamWriter.WriteLine($"{DateTime.Now:HH:mm} | {tipo} | {text}");

                            if (ex != null)
                            {
                                streamWriter.WriteLine($"{DateTime.Now:HH:mm} | ERROR  | {ex.Message}");
                                streamWriter.WriteLine($"{DateTime.Now:HH:mm} | STACK  | {ex.StackTrace}");
                                streamWriter.WriteLine($"{DateTime.Now:HH:mm} | SOURCE | {ex.Source}");

                                if (ex.InnerException != null)
                                {
                                    streamWriter.WriteLine($"{DateTime.Now:HH:mm} | ERROR  | {ex.InnerException.Message}");
                                    streamWriter.WriteLine($"{DateTime.Now:HH:mm} | STACK  | {ex.InnerException.StackTrace}");
                                    streamWriter.WriteLine($"{DateTime.Now:HH:mm} | SOURCE | {ex.InnerException.Source}");
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //Ignore
                }
            });
        }
    }
}