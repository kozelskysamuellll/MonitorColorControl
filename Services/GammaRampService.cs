using System;
using System.Runtime.InteropServices;
using MonitorColorControl.Models;

namespace MonitorColorControl.Services
{
    /// <summary>
    /// Service for managing monitor gamma ramps using Windows API
    /// </summary>
    public class GammaRampService
    {
        // Windows API declarations
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool GetDeviceGammaRamp(IntPtr hDC, IntPtr lpRamp);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool SetDeviceGammaRamp(IntPtr hDC, IntPtr lpRamp);

        private const int GAMMA_RAMP_SIZE = 256;
        private IntPtr? _deviceContext;
        private ushort[] _originalGammaRamp;

        public GammaRampService()
        {
            _originalGammaRamp = new ushort[GAMMA_RAMP_SIZE * 3];
            InitializeDeviceContext();
            CaptureOriginalGammaRamp();
        }

        /// <summary>
        /// Initializes the device context for the primary monitor
        /// </summary>
        private void InitializeDeviceContext()
        {
            try
            {
                _deviceContext = GetDC(IntPtr.Zero);
                if (_deviceContext == IntPtr.Zero)
                {
                    throw new Exception("Failed to get device context for primary display");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing device context: {ex.Message}");
            }
        }

        /// <summary>
        /// Captures the original gamma ramp from the display
        /// </summary>
        private void CaptureOriginalGammaRamp()
        {
            if (_deviceContext == null || _deviceContext == IntPtr.Zero)
                return;

            try
            {
                IntPtr gammaRampPtr = Marshal.AllocHGlobal(sizeof(ushort) * GAMMA_RAMP_SIZE * 3);
                if (GetDeviceGammaRamp(_deviceContext.Value, gammaRampPtr))
                {
                    Marshal.Copy(gammaRampPtr, _originalGammaRamp, 0, GAMMA_RAMP_SIZE * 3);
                }
                Marshal.FreeHGlobal(gammaRampPtr);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error capturing original gamma ramp: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies color settings to the display
        /// </summary>
        public bool ApplySettings(ColorSettings settings)
        {
            if (_deviceContext == null || _deviceContext == IntPtr.Zero)
                return false;

            try
            {
                // Create gamma ramp array
                ushort[] gammaRamp = new ushort[GAMMA_RAMP_SIZE * 3];

                // Calculate gamma values
                double gamma = settings.Gamma;
                double brightness = settings.Brightness;
                double contrast = settings.Contrast;
                double redChannel = 1.0 + (settings.RedChannel / 100.0);
                double greenChannel = 1.0 + (settings.GreenChannel / 100.0);
                double blueChannel = 1.0 + (settings.BlueChannel / 100.0);
                double saturation = settings.Saturation / 100.0;
                double vibrance = settings.Vibrance / 100.0;

                // Generate gamma ramp for each color channel
                for (int i = 0; i < GAMMA_RAMP_SIZE; i++)
                {
                    // Normalize input (0-255 to 0-1)
                    double normalized = i / 255.0;

                    // Apply contrast
                    double contrasted = (normalized - 0.5) * (contrast / 100.0) + 0.5;

                    // Apply gamma
                    double gammaApplied = Math.Pow(Math.Max(0, contrasted), 1.0 / gamma);

                    // Apply brightness
                    double brightened = gammaApplied + (brightness / 300.0);

                    // Clamp to valid range
                    brightened = Math.Max(0, Math.Min(1.0, brightened));

                    // Red channel
                    double red = brightened * redChannel * saturation * (1.0 + vibrance * 0.5);
                    gammaRamp[i] = (ushort)(Math.Max(0, Math.Min(1.0, red)) * 65535);

                    // Green channel
                    double green = brightened * greenChannel * saturation * (1.0 + vibrance * 0.5);
                    gammaRamp[GAMMA_RAMP_SIZE + i] = (ushort)(Math.Max(0, Math.Min(1.0, green)) * 65535);

                    // Blue channel
                    double blue = brightened * blueChannel * saturation * (1.0 + vibrance * 0.3);
                    gammaRamp[GAMMA_RAMP_SIZE * 2 + i] = (ushort)(Math.Max(0, Math.Min(1.0, blue)) * 65535);
                }

                // Apply gamma ramp
                IntPtr gammaRampPtr = Marshal.AllocHGlobal(sizeof(ushort) * GAMMA_RAMP_SIZE * 3);
                Marshal.Copy(gammaRamp, 0, gammaRampPtr, GAMMA_RAMP_SIZE * 3);
                bool result = SetDeviceGammaRamp(_deviceContext.Value, gammaRampPtr);
                Marshal.FreeHGlobal(gammaRampPtr);

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying settings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resets the display to original gamma ramp
        /// </summary>
        public bool ResetGammaRamp()
        {
            if (_deviceContext == null || _deviceContext == IntPtr.Zero)
                return false;

            try
            {
                IntPtr gammaRampPtr = Marshal.AllocHGlobal(sizeof(ushort) * GAMMA_RAMP_SIZE * 3);
                Marshal.Copy(_originalGammaRamp, 0, gammaRampPtr, GAMMA_RAMP_SIZE * 3);
                bool result = SetDeviceGammaRamp(_deviceContext.Value, gammaRampPtr);
                Marshal.FreeHGlobal(gammaRampPtr);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting gamma ramp: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public void Dispose()
        {
            try
            {
                ResetGammaRamp();
                if (_deviceContext != null && _deviceContext != IntPtr.Zero)
                {
                    ReleaseDC(IntPtr.Zero, _deviceContext.Value);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        ~GammaRampService()
        {
            Dispose();
        }
    }
}