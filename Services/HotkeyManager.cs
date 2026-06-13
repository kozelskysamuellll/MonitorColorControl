using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MonitorColorControl.Services
{
    /// <summary>
    /// Service for managing global hotkeys
    /// </summary>
    public class HotkeyManager
    {
        private const int WM_HOTKEY = 0x0312;
        private int _hotkeyId = 0;
        private Dictionary<int, Action> _hotkeyCallbacks = new Dictionary<int, Action>();

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        // Modifier keys
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        private IntPtr _windowHandle;
        private HwndSource? _hwndSource;

        public HotkeyManager(Window window)
        {
            try
            {
                _windowHandle = new WindowInteropHelper(window).Handle;
                _hwndSource = HwndSource.FromHwnd(_windowHandle);
                _hwndSource?.AddHook(HotkeyWndProc);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing hotkey manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a hotkey with a callback
        /// </summary>
        public bool RegisterHotkey(string hotkey, Action callback)
        {
            try
            {
                var (modifiers, vk) = ParseHotkey(hotkey);
                int id = ++_hotkeyId;

                if (RegisterHotKey(_windowHandle, id, modifiers, vk))
                {
                    _hotkeyCallbacks[id] = callback;
                    System.Diagnostics.Debug.WriteLine($"Hotkey registered: {hotkey} -> ID {id}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to register hotkey: {hotkey}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering hotkey: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unregisters a hotkey
        /// </summary>
        public bool UnregisterHotkey(int id)
        {
            try
            {
                if (UnregisterHotKey(_windowHandle, id))
                {
                    _hotkeyCallbacks.Remove(id);
                    System.Diagnostics.Debug.WriteLine($"Hotkey unregistered: ID {id}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error unregistering hotkey: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Parses hotkey string (e.g., "Ctrl+Alt+1") into modifier and key code
        /// </summary>
        private (uint, uint) ParseHotkey(string hotkey)
        {
            uint modifiers = 0;
            uint vk = 0;

            string[] parts = hotkey.Split('+');
            foreach (string part in parts)
            {
                string normalized = part.Trim().ToLower();
                switch (normalized)
                {
                    case "ctrl":
                    case "control":
                        modifiers |= MOD_CTRL;
                        break;
                    case "alt":
                        modifiers |= MOD_ALT;
                        break;
                    case "shift":
                        modifiers |= MOD_SHIFT;
                        break;
                    case "win":
                        modifiers |= MOD_WIN;
                        break;
                    default:
                        vk = GetVirtualKeyCode(normalized);
                        break;
                }
            }

            return (modifiers, vk);
        }

        /// <summary>
        /// Converts key name to virtual key code
        /// </summary>
        private uint GetVirtualKeyCode(string keyName)
        {
            return keyName switch
            {
                "0" => 0x30,
                "1" => 0x31,
                "2" => 0x32,
                "3" => 0x33,
                "4" => 0x34,
                "5" => 0x35,
                "6" => 0x36,
                "7" => 0x37,
                "8" => 0x38,
                "9" => 0x39,
                "a" => 0x41,
                "b" => 0x42,
                "c" => 0x43,
                "d" => 0x44,
                "e" => 0x45,
                "f" => 0x46,
                "g" => 0x47,
                "h" => 0x48,
                "i" => 0x49,
                "j" => 0x4A,
                "k" => 0x4B,
                "l" => 0x4C,
                "m" => 0x4D,
                "n" => 0x4E,
                "o" => 0x4F,
                "p" => 0x50,
                "q" => 0x51,
                "r" => 0x52,
                "s" => 0x53,
                "t" => 0x54,
                "u" => 0x55,
                "v" => 0x56,
                "w" => 0x57,
                "x" => 0x58,
                "y" => 0x59,
                "z" => 0x5A,
                "f1" => 0x70,
                "f2" => 0x71,
                "f3" => 0x72,
                "f4" => 0x73,
                "f5" => 0x74,
                "f6" => 0x75,
                "f7" => 0x76,
                "f8" => 0x77,
                "f9" => 0x78,
                "f10" => 0x79,
                "f11" => 0x7A,
                "f12" => 0x7B,
                _ => 0
            };
        }

        /// <summary>
        /// Window procedure for handling hotkey messages
        /// </summary>
        private IntPtr HotkeyWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (_hotkeyCallbacks.TryGetValue(id, out var callback))
                {
                    callback?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            try
            {
                var ids = new List<int>(_hotkeyCallbacks.Keys);
                foreach (var id in ids)
                {
                    UnregisterHotkey(id);
                }

                if (_hwndSource != null)
                {
                    _hwndSource.RemoveHook(HotkeyWndProc);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}