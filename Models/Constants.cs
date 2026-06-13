namespace MonitorColorControl.Models
{
    /// <summary>
    /// Application-wide constants and default values
    /// </summary>
    public static class Constants
    {
        // Application paths
        public static string AppDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MonitorColorControl"
        );

        public static string ProfilesPath => Path.Combine(AppDataPath, "profiles");
        public static string PresetsPath => Path.Combine(AppDataPath, "presets");
        public static string SettingsFile => Path.Combine(AppDataPath, "settings.json");

        // Slider ranges - Normal Mode
        public static class NormalMode
        {
            public const double GammaMin = 0.1;
            public const double GammaMax = 5.0;

            public const double BrightnessMin = -100;
            public const double BrightnessMax = 300;

            public const double ContrastMin = 0;
            public const double ContrastMax = 300;

            public const double SaturationMin = 0;
            public const double SaturationMax = 500;

            public const double VibranceMin = 0;
            public const double VibranceMax = 500;

            public const double ColorTempMin = 3000;
            public const double ColorTempMax = 10000;

            public const double RGBChannelMin = -100;
            public const double RGBChannelMax = 100;

            public const double BlackLevelMin = 0;
            public const double BlackLevelMax = 100;

            public const double WhiteLevelMin = 0;
            public const double WhiteLevelMax = 100;

            public const double HueMin = -180;
            public const double HueMax = 180;
        }

        // Slider ranges - Extreme Mode
        public static class ExtremeMode
        {
            public const double GammaMin = 0.1;
            public const double GammaMax = 10.0;

            public const double SaturationMin = 0;
            public const double SaturationMax = 1000;

            public const double VibranceMin = 0;
            public const double VibranceMax = 1000;
        }

        // Default hotkeys
        public static class DefaultHotkeys
        {
            public const string CompetitiveFPS = "Ctrl+Alt+1";
            public const string FortniteColorBoost = "Ctrl+Alt+2";
            public const string CS2VisibilityBoost = "Ctrl+Alt+3";
            public const string MovieMode = "Ctrl+Alt+4";
            public const string NightMode = "Ctrl+Alt+5";
            public const string Reset = "Ctrl+Alt+R";
        }

        // Profile categories
        public static class Categories
        {
            public const string Gaming = "Gaming";
            public const string Movies = "Movies";
            public const string Work = "Work";
            public const string Custom = "Custom";
            public const string Preset = "Preset";
        }

        // Preset profiles
        public static class Presets
        {
            public static class CompetitiveFPS
            {
                public const double Gamma = 1.2;
                public const double Brightness = 15;
                public const double Contrast = 120;
                public const double Saturation = 130;
                public const double Vibrance = 80;
            }

            public static class FortniteColorBoost
            {
                public const double Gamma = 1.1;
                public const double Brightness = 10;
                public const double Contrast = 130;
                public const double Saturation = 150;
                public const double Vibrance = 100;
                public const double ColorTemperature = 5500;
            }

            public static class CS2VisibilityBoost
            {
                public const double Gamma = 1.15;
                public const double Brightness = 20;
                public const double Contrast = 140;
                public const double Saturation = 120;
                public const double Vibrance = 90;
            }

            public static class MovieMode
            {
                public const double Gamma = 0.95;
                public const double Brightness = -5;
                public const double Contrast = 110;
                public const double Saturation = 110;
                public const double Vibrance = 20;
                public const double ColorTemperature = 6800;
            }

            public static class NightMode
            {
                public const double Gamma = 0.9;
                public const double Brightness = -10;
                public const double Contrast = 105;
                public const double Saturation = 95;
                public const double Vibrance = 0;
                public const double ColorTemperature = 3500;
                public const double BlueChannel = -20;
            }
        }

        // UI/UX
        public const int SliderUpdateDelayMs = 50;  // Debounce slider updates
        public const int PreviewUpdateDelayMs = 100; // Preview update delay
        public const string AppTitle = "Monitor Color Control";
        public const string AppVersion = "1.0.0";
    }
}