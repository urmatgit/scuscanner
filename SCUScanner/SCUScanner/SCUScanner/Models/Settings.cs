using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Models
{
    public static class  Settings
    {

        private static ISettings AppSettings =>  CrossSettings.Current;
        public static bool ContinuousScan
        {
            get => AppSettings.GetValueOrDefault(nameof(ContinuousScan), true);
            set => AppSettings.AddOrUpdateValue(nameof(ContinuousScan), value);
        }
        public static bool ManualScan
        {
            get => AppSettings.GetValueOrDefault(nameof(ManualScan), false);
            set => AppSettings.AddOrUpdateValue(nameof(ManualScan), value);
        }
        public static string SelectedLanguageCode
        {
            get => AppSettings.GetValueOrDefault(nameof(SelectedLanguageCode), "");
            set => AppSettings.AddOrUpdateValue(nameof(SelectedLanguageCode), value);
        }
    }
}

