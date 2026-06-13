using System;
using System.Collections.Generic;

namespace MonitorColorControl.Models
{
    /// <summary>
    /// Represents a saved color profile with metadata
    /// </summary>
    public class ColorProfile
    {
        public string Name { get; set; } = "Default Profile";
        public string Description { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public string Category { get; set; } = "Custom";  // e.g., "Gaming", "Movie", "Work", "Custom"
        
        // Settings for each monitor
        public Dictionary<string, ColorSettings> MonitorSettings { get; set; } = 
            new Dictionary<string, ColorSettings>();

        // Apply to all monitors flag
        public bool ApplyToAllMonitors { get; set; } = false;

        // Hotkey assignment (e.g., "Ctrl+Alt+1")
        public string? AssignedHotkey { get; set; }

        // Auto-apply on startup
        public bool AutoApplyOnStartup { get; set; } = false;

        /// <summary>
        /// Creates a deep copy of the profile
        /// </summary>
        public ColorProfile Clone()
        {
            var profile = new ColorProfile
            {
                Name = this.Name,
                Description = this.Description,
                CreatedDate = this.CreatedDate,
                LastModifiedDate = this.LastModifiedDate,
                Category = this.Category,
                ApplyToAllMonitors = this.ApplyToAllMonitors,
                AssignedHotkey = this.AssignedHotkey,
                AutoApplyOnStartup = this.AutoApplyOnStartup
            };

            // Deep copy monitor settings
            foreach (var kvp in this.MonitorSettings)
            {
                profile.MonitorSettings[kvp.Key] = kvp.Value.Clone();
            }

            return profile;
        }
    }
}