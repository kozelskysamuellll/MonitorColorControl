using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MonitorColorControl.Models;
using MonitorColorControl.Services;

namespace MonitorColorControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GammaRampService? _gammaRampService;
        private ProfileManager? _profileManager;
        private MonitorService? _monitorService;
        private HotkeyManager? _hotkeyManager;
        private ColorSettings _currentSettings;
        private ColorSettings? _previousSettings;
        private bool _isInitializing = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            LoadProfiles();
            SetupHotkeys();
            ApplyDefaultSettings();
            _isInitializing = false;
        }

        /// <summary>
        /// Initializes all services
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                _gammaRampService = new GammaRampService();
                _profileManager = new ProfileManager();
                _monitorService = new MonitorService();
                _hotkeyManager = new HotkeyManager(this);
                _currentSettings = new ColorSettings();

                // Create default presets if they don't exist
                _profileManager.CreateDefaultPresets();

                // Load monitors
                var monitors = _monitorService.GetMonitors();
                foreach (var monitor in monitors)
                {
                    MonitorComboBox.Items.Add(monitor.Name);
                }

                if (MonitorComboBox.Items.Count > 0)
                    MonitorComboBox.SelectedIndex = 0;

                UpdateStatus("Services initialized successfully");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error initializing services: {ex.Message}", isError: true);
            }
        }

        /// <summary>
        /// Applies current color settings to the display
        /// </summary>
        private void ApplyCurrentSettings()
        {
            try
            {
                if (_gammaRampService == null) return;

                // Save previous settings for undo
                _previousSettings = _currentSettings.Clone();

                // Apply settings
                if (_gammaRampService.ApplySettings(_currentSettings))
                {
                    UpdatePreviewColor();
                    UpdateStatus("Settings applied successfully");
                }
                else
                {
                    UpdateStatus("Failed to apply settings", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error applying settings: {ex.Message}", isError: true);
            }
        }

        /// <summary>
        /// Updates the preview color box
        /// </summary>
        private void UpdatePreviewColor()
        {
            try
            {
                // Create a preview color based on current settings
                double r = 128 + (_currentSettings.RedChannel * 1.27);
                double g = 128 + (_currentSettings.GreenChannel * 1.27);
                double b = 128 + (_currentSettings.BlueChannel * 1.27);

                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));

                Color previewColor = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
                PreviewBox.Background = new SolidColorBrush(previewColor);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating preview: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all profiles from disk and populates the listbox
        /// </summary>
        private void LoadProfiles()
        {
            try
            {
                if (_profileManager == null) return;

                ProfileListBox.Items.Clear();
                var profiles = _profileManager.GetAllProfiles();

                foreach (var profile in profiles)
                {
                    ProfileListBox.Items.Add(profile.Name);
                }

                if (ProfileListBox.Items.Count > 0)
                    ProfileListBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading profiles: {ex.Message}", isError: true);
            }
        }

        /// <summary>
        /// Sets up global hotkeys
        /// </summary>
        private void SetupHotkeys()
        {
            try
            {
                if (_hotkeyManager == null) return;

                _hotkeyManager.RegisterHotkey("Ctrl+Alt+1", () => LoadProfileByName("FPS Competitive"));
                _hotkeyManager.RegisterHotkey("Ctrl+Alt+2", () => LoadProfileByName("Fortnite Color Boost"));
                _hotkeyManager.RegisterHotkey("Ctrl+Alt+3", () => LoadProfileByName("CS2 Visibility Boost"));
                _hotkeyManager.RegisterHotkey("Ctrl+Alt+4", () => LoadProfileByName("Movie Mode"));
                _hotkeyManager.RegisterHotkey("Ctrl+Alt+5", () => LoadProfileByName("Night Mode"));
                _hotkeyManager.RegisterHotkey("Ctrl+Alt+R", ResetToDefault);

                UpdateStatus("Hotkeys registered");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error setting up hotkeys: {ex.Message}", isError: true);
            }
        }

        /// <summary>
        /// Loads a profile by name and applies it
        /// </summary>
        private void LoadProfileByName(string profileName)
        {
            try
            {
                if (_profileManager == null) return;

                var profile = _profileManager.LoadProfile(profileName);
                if (profile != null)
                {
                    ApplyProfile(profile);
                    UpdateStatus($"Loaded profile: {profileName}");
                }
                else
                {
                    UpdateStatus($"Profile not found: {profileName}", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading profile: {ex.Message}", isError: true);
            }
        }

        /// <summary>
        /// Applies a profile to the UI and settings
        /// </summary>
        private void ApplyProfile(ColorProfile profile)
        {
            try
            {
                _isInitializing = true;

                // Get settings for primary monitor
                if (profile.MonitorSettings.TryGetValue("Display 1", out var settings))
                {
                    _currentSettings = settings.Clone();
                    UpdateUIFromSettings();
                    ApplyCurrentSettings();
                }

                _isInitializing = false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error applying profile: {ex.Message}", isError: true);
                _isInitializing = false;
            }
        }

        /// <summary>
        /// Updates UI sliders to match current settings
        /// </summary>
        private void UpdateUIFromSettings()
        {
            GammaSlider.Value = _currentSettings.Gamma;
            BrightnessSlider.Value = _currentSettings.Brightness;
            ContrastSlider.Value = _currentSettings.Contrast;
            SaturationSlider.Value = _currentSettings.Saturation;
            VibranceSlider.Value = _currentSettings.Vibrance;
            ColorTempSlider.Value = _currentSettings.ColorTemperature;
            RedSlider.Value = _currentSettings.RedChannel;
            GreenSlider.Value = _currentSettings.GreenChannel;
            BlueSlider.Value = _currentSettings.BlueChannel;
            BlackLevelSlider.Value = _currentSettings.BlackLevel;
            WhiteLevelSlider.Value = _currentSettings.WhiteLevel;
            ExtremeModeCheckBox.IsChecked = _currentSettings.ExtremeMode;
        }

        /// <summary>
        /// Updates settings from UI sliders
        /// </summary>
        private void UpdateSettingsFromUI()
        {
            _currentSettings.Gamma = GammaSlider.Value;
            _currentSettings.Brightness = BrightnessSlider.Value;
            _currentSettings.Contrast = ContrastSlider.Value;
            _currentSettings.Saturation = SaturationSlider.Value;
            _currentSettings.Vibrance = VibranceSlider.Value;
            _currentSettings.ColorTemperature = ColorTempSlider.Value;
            _currentSettings.RedChannel = RedSlider.Value;
            _currentSettings.GreenChannel = GreenSlider.Value;
            _currentSettings.BlueChannel = BlueSlider.Value;
            _currentSettings.BlackLevel = BlackLevelSlider.Value;
            _currentSettings.WhiteLevel = WhiteLevelSlider.Value;
            _currentSettings.ExtremeMode = ExtremeModeCheckBox.IsChecked ?? false;
        }

        /// <summary>
        /// Applies default settings on startup
        /// </summary>
        private void ApplyDefaultSettings()
        {
            _currentSettings = new ColorSettings();
            UpdateUIFromSettings();
            ApplyCurrentSettings();
        }

        /// <summary>
        /// Resets to default settings
        /// </summary>
        private void ResetToDefault()
        {
            try
            {
                _isInitializing = true;
                _currentSettings.Reset();
                UpdateUIFromSettings();
                ApplyCurrentSettings();
                UpdateStatus("Reset to default settings");
                _isInitializing = false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error resetting: {ex.Message}", isError: true);
                _isInitializing = false;
            }
        }

        /// <summary>
        /// Updates status message
        /// </summary>
        private void UpdateStatus(string message, bool isError = false)
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = message;
                StatusTextBlock.Foreground = new SolidColorBrush(isError ? Color.FromRgb(220, 53, 69) : Color.FromRgb(40, 167, 69));
            });
        }

        // UI Event Handlers

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;

            UpdateSettingsFromUI();
            UpdateValueLabels();
            UpdatePreviewColor();
        }

        private void UpdateValueLabels()
        {
            GammaValue.Text = GammaSlider.Value.ToString("F2");
            BrightnessValue.Text = BrightnessSlider.Value.ToString("F0");
            ContrastValue.Text = ContrastSlider.Value.ToString("F0");
            SaturationValue.Text = SaturationSlider.Value.ToString("F0") + "%";
            VibranceValue.Text = VibranceSlider.Value.ToString("F0") + "%";
            ColorTempValue.Text = ColorTempSlider.Value.ToString("F0");
            RedValue.Text = RedSlider.Value.ToString("F0");
            GreenValue.Text = GreenSlider.Value.ToString("F0");
            BlueValue.Text = BlueSlider.Value.ToString("F0");
            BlackLevelValue.Text = BlackLevelSlider.Value.ToString("F0");
            WhiteLevelValue.Text = WhiteLevelSlider.Value.ToString("F0");
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProfileNameTextBox.Text))
                {
                    UpdateStatus("Please enter a profile name", isError: true);
                    return;
                }

                if (_profileManager == null) return;

                UpdateSettingsFromUI();

                var profile = new ColorProfile
                {
                    Name = ProfileNameTextBox.Text,
                    Category = CategoryComboBox.SelectedItem?.ToString() ?? "Custom",
                    Description = "Custom profile"
                };
                profile.MonitorSettings["Display 1"] = _currentSettings.Clone();

                if (_profileManager.SaveProfile(profile))
                {
                    LoadProfiles();
                    ProfileNameTextBox.Clear();
                    UpdateStatus($"Profile saved: {profile.Name}");
                }
                else
                {
                    UpdateStatus("Failed to save profile", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error saving profile: {ex.Message}", isError: true);
            }
        }

        private void LoadProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProfileListBox.SelectedItem is string profileName)
                {
                    LoadProfileByName(profileName);
                }
                else
                {
                    UpdateStatus("Please select a profile", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading profile: {ex.Message}", isError: true);
            }
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProfileListBox.SelectedItem is string profileName)
                {
                    if (_profileManager?.DeleteProfile(profileName) ?? false)
                    {
                        LoadProfiles();
                        UpdateStatus($"Profile deleted: {profileName}");
                    }
                    else
                    {
                        UpdateStatus("Failed to delete profile", isError: true);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error deleting profile: {ex.Message}", isError: true);
            }
        }

        private void ImportProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".json",
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Import Profile"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var profile = _profileManager?.ImportProfile(openFileDialog.FileName);
                    if (profile != null)
                    {
                        _profileManager?.SaveProfile(profile);
                        LoadProfiles();
                        UpdateStatus($"Profile imported: {profile.Name}");
                    }
                    else
                    {
                        UpdateStatus("Failed to import profile", isError: true);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error importing profile: {ex.Message}", isError: true);
            }
        }

        private void ExportProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProfileListBox.SelectedItem is string profileName)
                {
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        DefaultExt = ".json",
                        Filter = "JSON Files (*.json)|*.json",
                        FileName = profileName + ".json"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var profile = _profileManager?.LoadProfile(profileName);
                        if (profile != null && _profileManager?.ExportProfile(profile, saveFileDialog.FileName) == true)
                        {
                            UpdateStatus($"Profile exported: {profileName}");
                        }
                        else
                        {
                            UpdateStatus("Failed to export profile", isError: true);
                        }
                    }
                }
                else
                {
                    UpdateStatus("Please select a profile to export", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error exporting profile: {ex.Message}", isError: true);
            }
        }

        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettingsFromUI();
            ApplyCurrentSettings();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetToDefault();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_previousSettings != null)
                {
                    _currentSettings = _previousSettings.Clone();
                    UpdateUIFromSettings();
                    ApplyCurrentSettings();
                    UpdateStatus("Undo successful");
                }
                else
                {
                    UpdateStatus("Nothing to undo", isError: true);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error undoing: {ex.Message}", isError: true);
            }
        }

        private void ExtremeMode_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            _currentSettings.ExtremeMode = true;
            GammaSlider.Maximum = 10.0;
            SaturationSlider.Maximum = 1000;
            VibranceSlider.Maximum = 1000;
            UpdateStatus("Extreme Mode enabled");
        }

        private void ExtremeMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            _currentSettings.ExtremeMode = false;
            GammaSlider.Maximum = 5.0;
            SaturationSlider.Maximum = 500;
            VibranceSlider.Maximum = 500;

            // Clamp values if they exceed normal mode limits
            if (GammaSlider.Value > 5.0) GammaSlider.Value = 5.0;
            if (SaturationSlider.Value > 500) SaturationSlider.Value = 500;
            if (VibranceSlider.Value > 500) VibranceSlider.Value = 500;

            UpdateStatus("Extreme Mode disabled");
        }

        private void ProfileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileListBox.SelectedItem is string profileName)
            {
                ProfileNameTextBox.Text = profileName;
            }
        }

        private void MonitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Monitor selection logic can be added here
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _hotkeyManager?.Dispose();
            _gammaRampService?.Dispose();
        }
    }
}