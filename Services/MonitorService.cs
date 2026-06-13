using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonitorColorControl.Services
{
    /// <summary>
    /// Service for detecting and managing multiple monitors
    /// </summary>
    public class MonitorService
    {
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
            EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        private delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        public class Monitor
        {
            public string Name { get; set; } = "Unknown";
            public string DeviceName { get; set; } = "";
            public int Index { get; set; }
            public bool IsPrimary { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private List<Monitor> _monitors = new List<Monitor>();

        public MonitorService()
        {
            RefreshMonitors();
        }

        /// <summary>
        /// Detects all connected monitors
        /// </summary>
        public void RefreshMonitors()
        {
            _monitors.Clear();
            int monitorIndex = 0;

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));

                    if (GetMonitorInfo(hMonitor, ref monitorInfo))
                    {
                        var monitor = new Monitor
                        {
                            Name = $"Display {monitorIndex + 1}",
                            DeviceName = monitorInfo.szDevice,
                            Index = monitorIndex,
                            IsPrimary = (monitorInfo.dwFlags & 1) != 0,
                            Left = monitorInfo.rcMonitor.left,
                            Top = monitorInfo.rcMonitor.top,
                            Width = monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left,
                            Height = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top
                        };

                        _monitors.Add(monitor);
                        monitorIndex++;
                    }

                    return true;
                }, IntPtr.Zero);
        }

        /// <summary>
        /// Gets all connected monitors
        /// </summary>
        public List<Monitor> GetMonitors()
        {
            return new List<Monitor>(_monitors);
        }

        /// <summary>
        /// Gets the primary monitor
        /// </summary>
        public Monitor? GetPrimaryMonitor()
        {
            return _monitors.Find(m => m.IsPrimary);
        }

        /// <summary>
        /// Gets a monitor by index
        /// </summary>
        public Monitor? GetMonitorByIndex(int index)
        {
            return index >= 0 && index < _monitors.Count ? _monitors[index] : null;
        }

        /// <summary>
        /// Gets monitor count
        /// </summary>
        public int GetMonitorCount()
        {
            return _monitors.Count;
        }
    }
}