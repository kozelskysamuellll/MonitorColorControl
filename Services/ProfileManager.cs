using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MonitorColorControl.Models;

namespace MonitorColorControl.Services
{
    /// <summary>
    /// Service for saving and loading color profiles from disk
    /// </summary>
    public class ProfileManager
    {
        private readonly string _profilesPath;
        private readonly JsonSerializerSettings _jsonSettings;

        public ProfileManager()
        {
            _profilesPath = Constants.ProfilesPath;
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            EnsureDirectoriesExist();
        }

        /// <summary>
        /// Ensures required directories exist
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            try
            {
                if (!Directory.Exists(Constants.AppDataPath))
                    Directory.CreateDirectory(Constants.AppDataPath);

                if (!Directory.Exists(_profilesPath))
                    Directory.CreateDirectory(_profilesPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating directories: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves a profile to disk
        /// </summary>
        public bool SaveProfile(ColorProfile profile)
        {
            try
            {
                EnsureDirectoriesExist();
                profile.LastModifiedDate = DateTime.Now;

                string filename = SanitizeFilename(profile.Name) + ".json";
                string filepath = Path.Combine(_profilesPath, filename);

                string json = JsonConvert.SerializeObject(profile, _jsonSettings);
                File.WriteAllText(filepath, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads a profile from disk
        /// </summary>
        public ColorProfile? LoadProfile(string profileName)
        {
            try
            {
                string filename = SanitizeFilename(profileName) + ".json";
                string filepath = Path.Combine(_profilesPath, filename);

                if (!File.Exists(filepath))
                    return null;

                string json = File.ReadAllText(filepath);
                return JsonConvert.DeserializeObject<ColorProfile>(json, _jsonSettings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets all saved profiles
        /// </summary>
        public List<ColorProfile> GetAllProfiles()
        {
            var profiles = new List<ColorProfile>();

            try
            {
                EnsureDirectoriesExist();

                if (!Directory.Exists(_profilesPath))
                    return profiles;

                var files = Directory.GetFiles(_profilesPath, "*.json");
                foreach (var file in files)
                {
                    try
                    {
                        string json = File.ReadAllText(file);
                        var profile = JsonConvert.DeserializeObject<ColorProfile>(json, _jsonSettings);
                        if (profile != null)
                            profiles.Add(profile);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading profile {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting profiles: {ex.Message}");
            }

            return profiles.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Deletes a profile
        /// </summary>
        public bool DeleteProfile(string profileName)
        {
            try
            {
                string filename = SanitizeFilename(profileName) + ".json";
                string filepath = Path.Combine(_profilesPath, filename);

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports a profile to a specified location
        /// </summary>
        public bool ExportProfile(ColorProfile profile, string filepath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(profile, _jsonSettings);
                File.WriteAllText(filepath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Imports a profile from a file
        /// </summary>
        public ColorProfile? ImportProfile(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                    return null;

                string json = File.ReadAllText(filepath);
                return JsonConvert.DeserializeObject<ColorProfile>(json, _jsonSettings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing profile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sanitizes filename to be valid on Windows
        /// </summary>
        private string SanitizeFilename(string filename)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                filename = filename.Replace(c.ToString(), "_");
            }
            return filename.Length > 250 ? filename.Substring(0, 250) : filename;
        }

        /// <summary>
        /// Creates default preset profiles
        /// </summary>
        public void CreateDefaultPresets()
        {
            try
            {
                // FPS Competitive
                var competitiveFPS = new ColorProfile
                {
                    Name = "FPS Competitive",
                    Description = "Optimized for competitive FPS games with enhanced visibility",
                    Category = Constants.Categories.Gaming,
                    AutoApplyOnStartup = false
                };
                competitiveFPS.MonitorSettings["Display 1"] = new ColorSettings
                {
                    Gamma = Constants.Presets.CompetitiveFPS.Gamma,
                    Brightness = Constants.Presets.CompetitiveFPS.Brightness,
                    Contrast = Constants.Presets.CompetitiveFPS.Contrast,
                    Saturation = Constants.Presets.CompetitiveFPS.Saturation,
                    Vibrance = Constants.Presets.CompetitiveFPS.Vibrance,
                    MonitorId = "Display 1"
                };
                SaveProfile(competitiveFPS);

                // Fortnite Color Boost
                var fortniteBoost = new ColorProfile
                {
                    Name = "Fortnite Color Boost",
                    Description = "Vibrant colors for better enemy visibility in Fortnite",
                    Category = Constants.Categories.Gaming,
                    AutoApplyOnStartup = false
                };
                fortniteBoost.MonitorSettings["Display 1"] = new ColorSettings
                {
                    Gamma = Constants.Presets.FortniteColorBoost.Gamma,
                    Brightness = Constants.Presets.FortniteColorBoost.Brightness,
                    Contrast = Constants.Presets.FortniteColorBoost.Contrast,
                    Saturation = Constants.Presets.FortniteColorBoost.Saturation,
                    Vibrance = Constants.Presets.FortniteColorBoost.Vibrance,
                    ColorTemperature = Constants.Presets.FortniteColorBoost.ColorTemperature,
                    MonitorId = "Display 1"
                };
                SaveProfile(fortniteBoost);

                // CS2 Visibility Boost
                var cs2Boost = new ColorProfile
                {
                    Name = "CS2 Visibility Boost",
                    Description = "Enhanced smoke and utility visibility for CS2",
                    Category = Constants.Categories.Gaming,
                    AutoApplyOnStartup = false
                };
                cs2Boost.MonitorSettings["Display 1"] = new ColorSettings
                {
                    Gamma = Constants.Presets.CS2VisibilityBoost.Gamma,
                    Brightness = Constants.Presets.CS2VisibilityBoost.Brightness,
                    Contrast = Constants.Presets.CS2VisibilityBoost.Contrast,
                    Saturation = Constants.Presets.CS2VisibilityBoost.Saturation,
                    Vibrance = Constants.Presets.CS2VisibilityBoost.Vibrance,
                    MonitorId = "Display 1"
                };
                SaveProfile(cs2Boost);

                // Movie Mode
                var movieMode = new ColorProfile
                {
                    Name = "Movie Mode",
                    Description = "Cinema-like color accuracy for movies and videos",
                    Category = Constants.Categories.Movies,
                    AutoApplyOnStartup = false
                };
                movieMode.MonitorSettings["Display 1"] = new ColorSettings
                {
                    Gamma = Constants.Presets.MovieMode.Gamma,
                    Brightness = Constants.Presets.MovieMode.Brightness,
                    Contrast = Constants.Presets.MovieMode.Contrast,
                    Saturation = Constants.Presets.MovieMode.Saturation,
                    Vibrance = Constants.Presets.MovieMode.Vibrance,
                    ColorTemperature = Constants.Presets.MovieMode.ColorTemperature,
                    MonitorId = "Display 1"
                };
                SaveProfile(movieMode);

                // Night Mode
                var nightMode = new ColorProfile
                {
                    Name = "Night Mode",
                    Description = "Blue light reduction for comfortable night viewing",
                    Category = Constants.Categories.Custom,
                    AutoApplyOnStartup = false
                };
                nightMode.MonitorSettings["Display 1"] = new ColorSettings
                {
                    Gamma = Constants.Presets.NightMode.Gamma,
                    Brightness = Constants.Presets.NightMode.Brightness,
                    Contrast = Constants.Presets.NightMode.Contrast,
                    Saturation = Constants.Presets.NightMode.Saturation,
                    Vibrance = Constants.Presets.NightMode.Vibrance,
                    ColorTemperature = Constants.Presets.NightMode.ColorTemperature,
                    BlueChannel = Constants.Presets.NightMode.BlueChannel,
                    MonitorId = "Display 1"
                };
                SaveProfile(nightMode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating default presets: {ex.Message}");
            }
        }
    }
}