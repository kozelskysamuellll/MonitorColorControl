using System;
using MonitorColorControl.Models;

namespace MonitorColorControl.Services
{
    /// <summary>
    /// Service for color transformation calculations
    /// </summary>
    public class ColorTransformService
    {
        /// <summary>
        /// Converts HSV color to RGB
        /// </summary>
        public static (byte R, byte G, byte B) HSVToRGB(double hue, double saturation, double value)
        {
            hue = ((hue % 360) + 360) % 360;
            saturation = Math.Max(0, Math.Min(1, saturation));
            value = Math.Max(0, Math.Min(1, value));

            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            double r, g, b;
            if (hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return (
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
        }

        /// <summary>
        /// Converts RGB to HSV
        /// </summary>
        public static (double H, double S, double V) RGBToHSV(byte r, byte g, byte b)
        {
            double rd = r / 255.0;
            double gd = g / 255.0;
            double bd = b / 255.0;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double delta = max - min;

            double hue = 0;
            if (delta != 0)
            {
                if (max == rd)
                    hue = 60 * (((gd - bd) / delta) % 6);
                else if (max == gd)
                    hue = 60 * (((bd - rd) / delta) + 2);
                else
                    hue = 60 * (((rd - gd) / delta) + 4);
            }

            double saturation = max == 0 ? 0 : delta / max;
            double value = max;

            return (hue, saturation, value);
        }

        /// <summary>
        /// Applies color temperature to RGB values
        /// </summary>
        public static (double R, double G, double B) ApplyColorTemperature(double kelvin)
        {
            kelvin = Math.Max(3000, Math.Min(10000, kelvin));
            
            // Normalize kelvin to 0-1 range
            double normalizedKelvin = (kelvin - 3000) / 7000.0;

            // Warm (warm red) to cool (cool blue)
            double red = 1.0;
            double green = 0.4 + (0.6 * normalizedKelvin);
            double blue = 0.2 + (0.8 * normalizedKelvin);

            return (red, green, blue);
        }

        /// <summary>
        /// Applies saturation adjustment to RGB
        /// </summary>
        public static (byte R, byte G, byte B) ApplySaturation(byte r, byte g, byte b, double saturation)
        {
            saturation = Math.Max(0, saturation / 100.0);
            
            var (h, s, v) = RGBToHSV(r, g, b);
            s = Math.Min(1.0, s * saturation);
            
            return HSVToRGB(h, s, v);
        }

        /// <summary>
        /// Applies vibrance (saturation boost for less saturated colors)
        /// </summary>
        public static (byte R, byte G, byte B) ApplyVibrance(byte r, byte g, byte b, double vibrance)
        {
            vibrance = Math.Max(0, vibrance / 100.0);
            
            var (h, s, v) = RGBToHSV(r, g, b);
            
            // Vibrance affects less saturated colors more
            double boost = (1.0 - s) * vibrance * 0.5;
            s = Math.Min(1.0, s + boost);
            
            return HSVToRGB(h, s, v);
        }

        /// <summary>
        /// Calculates contrast adjusted value
        /// </summary>
        public static byte ApplyContrast(byte value, double contrast)
        {
            contrast = Math.Max(0, contrast / 100.0);
            double normalized = value / 255.0;
            double adjusted = (normalized - 0.5) * contrast + 0.5;
            adjusted = Math.Max(0, Math.Min(1, adjusted));
            return (byte)(adjusted * 255);
        }

        /// <summary>
        /// Calculates brightness adjusted value
        /// </summary>
        public static byte ApplyBrightness(byte value, double brightness)
        {
            brightness = Math.Max(-100, Math.Min(300, brightness)) / 100.0;
            double normalized = value / 255.0;
            double adjusted = normalized + brightness;
            adjusted = Math.Max(0, Math.Min(1, adjusted));
            return (byte)(adjusted * 255);
        }

        /// <summary>
        /// Applies gamma correction
        /// </summary>
        public static byte ApplyGamma(byte value, double gamma)
        {
            gamma = Math.Max(0.1, Math.Min(10.0, gamma));
            double normalized = value / 255.0;
            double corrected = Math.Pow(normalized, 1.0 / gamma);
            return (byte)(corrected * 255);
        }

        /// <summary>
        /// Applies all color transformations to a single pixel
        /// </summary>
        public static (byte R, byte G, byte B) ApplyAllTransforms(
            byte r, byte g, byte b,
            ColorSettings settings)
        {
            // Apply gamma
            r = ApplyGamma(r, settings.Gamma);
            g = ApplyGamma(g, settings.Gamma);
            b = ApplyGamma(b, settings.Gamma);

            // Apply brightness
            r = ApplyBrightness(r, settings.Brightness);
            g = ApplyBrightness(g, settings.Brightness);
            b = ApplyBrightness(b, settings.Brightness);

            // Apply contrast
            r = ApplyContrast(r, settings.Contrast);
            g = ApplyContrast(g, settings.Contrast);
            b = ApplyContrast(b, settings.Contrast);

            // Apply saturation
            (r, g, b) = ApplySaturation(r, g, b, settings.Saturation);

            // Apply vibrance
            (r, g, b) = ApplyVibrance(r, g, b, settings.Vibrance);

            return (r, g, b);
        }
    }
}