using System;

namespace MonitorColorControl.Models
{
    /// <summary>
    /// Represents all color adjustment settings for a monitor
    /// </summary>
    public class ColorSettings
    {
        // Gamma control (0.1 to 5.0, extended to 10.0 in Extreme Mode)
        public double Gamma { get; set; } = 1.0;

        // Brightness (-100 to +300)
        public double Brightness { get; set; } = 0;

        // Contrast (0 to 300)
        public double Contrast { get; set; } = 100;

        // Saturation (0 to 500%, extended to 1000% in Extreme Mode)
        public double Saturation { get; set; } = 100;

        // Digital Vibrance (0 to 500%, extended to 1000% in Extreme Mode)
        public double Vibrance { get; set; } = 0;

        // Color Temperature in Kelvin (3000K to 10000K)
        public double ColorTemperature { get; set; } = 6500;

        // RGB Channel adjustments (-100 to +100)
        public double RedChannel { get; set; } = 0;
        public double GreenChannel { get; set; } = 0;
        public double BlueChannel { get; set; } = 0;

        // Black and White levels
        public double BlackLevel { get; set; } = 0;    // 0 to 100
        public double WhiteLevel { get; set; } = 100;  // 0 to 100

        // Hue shift (-180 to +180)
        public double Hue { get; set; } = 0;

        // Extreme Mode flag
        public bool ExtremeMode { get; set; } = false;

        // Monitor identifier (index or name)
        public string MonitorId { get; set; } = "Display 1";

        /// <summary>
        /// Creates a deep copy of the current settings
        /// </summary>
        public ColorSettings Clone()
        {
            return new ColorSettings
            {
                Gamma = this.Gamma,
                Brightness = this.Brightness,
                Contrast = this.Contrast,
                Saturation = this.Saturation,
                Vibrance = this.Vibrance,
                ColorTemperature = this.ColorTemperature,
                RedChannel = this.RedChannel,
                GreenChannel = this.GreenChannel,
                BlueChannel = this.BlueChannel,
                BlackLevel = this.BlackLevel,
                WhiteLevel = this.WhiteLevel,
                Hue = this.Hue,
                ExtremeMode = this.ExtremeMode,
                MonitorId = this.MonitorId
            };
        }

        /// <summary>
        /// Resets all settings to default values
        /// </summary>
        public void Reset()
        {
            Gamma = 1.0;
            Brightness = 0;
            Contrast = 100;
            Saturation = 100;
            Vibrance = 0;
            ColorTemperature = 6500;
            RedChannel = 0;
            GreenChannel = 0;
            BlueChannel = 0;
            BlackLevel = 0;
            WhiteLevel = 100;
            Hue = 0;
        }

        /// <summary>
        /// Validates settings are within acceptable ranges
        /// </summary>
        public bool Validate()
        {
            double gammaMax = ExtremeMode ? 10.0 : 5.0;
            double saturationMax = ExtremeMode ? 1000 : 500;
            double vibranceMax = ExtremeMode ? 1000 : 500;

            return Gamma >= 0.1 && Gamma <= gammaMax &&
                   Brightness >= -100 && Brightness <= 300 &&
                   Contrast >= 0 && Contrast <= 300 &&
                   Saturation >= 0 && Saturation <= saturationMax &&
                   Vibrance >= 0 && Vibrance <= vibranceMax &&
                   ColorTemperature >= 3000 && ColorTemperature <= 10000 &&
                   RedChannel >= -100 && RedChannel <= 100 &&
                   GreenChannel >= -100 && GreenChannel <= 100 &&
                   BlueChannel >= -100 && BlueChannel <= 100 &&
                   BlackLevel >= 0 && BlackLevel <= 100 &&
                   WhiteLevel >= 0 && WhiteLevel <= 100 &&
                   Hue >= -180 && Hue <= 180;
        }
    }
}