﻿using System;
using Android.App;
using Android.Content;
using Android.OS;
using VideoPlayerProima.Droid.Controls;

namespace VideoPlayerProima.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true, Name = "br.com.proima.VideoPlayerProima.Android.Droid.BootReceiver")]
    [IntentFilter(new[] {Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted})]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                bool bootCompleted;
                string action = intent.Action;

                LoggerService.Instance.Info("BootReceiver: OnReceive");
                LoggerService.Instance.Info($"BootReceiver: Action: {intent.Action}");

                if (Build.VERSION.SdkInt > BuildVersionCodes.M)
                    bootCompleted = Intent.ActionLockedBootCompleted == action;
                else
                    bootCompleted = Intent.ActionBootCompleted == action;

                LoggerService.Instance.Info($"BootReceiver: bootCompleted: {bootCompleted}");

                Intent serviceStart = new Intent(context, typeof(MainActivity));
                serviceStart.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(serviceStart);
            }
            catch (Exception ex)
            {
                LoggerService.Instance.Error($"BootReceiver: OnReceive: Error: {ex.Message}", ex);
            }
        }
    }
}