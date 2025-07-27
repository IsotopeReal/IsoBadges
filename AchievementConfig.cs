using BepInEx.Configuration;
using UnityEngine;

namespace IsoBadges
{
    public enum AchievementUIStyle
    {
        Default,
        Compact
    }
    public enum ProgressDisplayStyle
    {
        Ratio,
        Percentage
    }

    // Static class to hold all config entries
    public static class ConfigManager
    {
        // General Settings
        public static ConfigEntry<bool> ShowProgressPopups { get; private set; }
        public static ConfigEntry<AchievementUIStyle> UIStyle { get; private set; }
        public static ConfigEntry<ProgressDisplayStyle> ProgressDisplay { get; private set; }

        // Color Settings
        public static ConfigEntry<Color> PanelBaseColor { get; private set; }
        public static ConfigEntry<float> PanelTransparency { get; private set; }
        public static ConfigEntry<Color> UnlockedTextColor { get; private set; }
        public static ConfigEntry<Color> Default_TitleColor { get; private set; }
        public static ConfigEntry<Color> Default_DescriptionColor { get; private set; }
        public static ConfigEntry<Color> Compact_ProgressColor { get; private set; }

        // Default Style Settings
        public static ConfigEntry<int> Default_PanelWidth { get; private set; }
        public static ConfigEntry<int> Default_PanelHeight { get; private set; }
        public static ConfigEntry<int> Default_IconSize { get; private set; }
        public static ConfigEntry<int> Default_TitleFontSize { get; private set; }
        public static ConfigEntry<int> Default_DescriptionFontSize { get; private set; }

        // Compact Style Settings
        public static ConfigEntry<int> Compact_PanelWidth { get; private set; }
        public static ConfigEntry<int> Compact_PanelHeight { get; private set; }
        public static ConfigEntry<int> Compact_IconSize { get; private set; }
        public static ConfigEntry<int> Compact_ProgressFontSize { get; private set; }

        public static void Setup(ConfigFile config)
        {
            // General Section
            ShowProgressPopups = config.Bind("General", "ShowProgressPopups", true, "If true, a pop-up will appear every time progress is made on a badge. If false, it will only appear when the badge is fully unlocked.");
            UIStyle = config.Bind("General", "UIStyle", AchievementUIStyle.Default, "The visual style of the achievement pop-up.");
            ProgressDisplay = config.Bind("General", "ProgressDisplay", ProgressDisplayStyle.Ratio, "How progress is displayed. Options: Ratio (e.g., 50/100) or Percentage (e.g., 50%).");

            // Color Style Section
            PanelBaseColor = config.Bind("Style - Colors", "Panel Base Color", new Color(0.1f, 0.1f, 0.15f), "The RGB base color of the notification panel.");
            PanelTransparency = config.Bind("Style - Colors", "Panel Transparency", 0.95f, "The transparency (alpha) of the notification panel. 0 is fully transparent, 1 is fully opaque.");
            UnlockedTextColor = config.Bind("Style - Colors", "Unlocked Text Color", new Color(1f, 0.84f, 0f), "The color of the 'Achievement Unlocked' text.");

            // Default Style Section
            Default_TitleFontSize = config.Bind("Style - Default", "Default Style - Title Font Size", 16, "The font size for the achievement title for the Default style.");
            Default_DescriptionFontSize = config.Bind("Style - Default", "Default Style - Description Font Size", 14, "The font size for the achievement description and progress for the Default style.");
            Default_IconSize = config.Bind("Style - Default", "Default Style - Icon Size", 50, "The size (width and height) of the badge icon in pixels for the Default style.");
            Default_TitleColor = config.Bind("Style - Colors", "Default Style - Title Color", Color.white, "The color of the title text in the Default style.");
            Default_DescriptionColor = config.Bind("Style - Colors", "Default Style - Description Color", Color.gray, "The color of the description/progress text in the Default style.");
            Default_PanelWidth = config.Bind("Style - Default", "Default Style - Panel Width", 260, "The width of the notification panel in pixels for the Default style.");
            Default_PanelHeight = config.Bind("Style - Default", "Default Style - Panel Height", 70, "The height of the notification panel in pixels for the Default style.");
            Default_IconSize = config.Bind("Style - Default", "Default Style - Icon Size", 50, "The size (width and height) of the badge icon in pixels for the Default style.");

            // Compact Style Section
            Compact_ProgressFontSize = config.Bind("Style - Compact", "Compact Style - Progress Font Size", 14, "The font size for the progress/unlocked text for the Compact style.");
            Compact_IconSize = config.Bind("Style - Compact", "Compact Style - Icon Size", 60, "The size (width and height) of the badge icon in pixels for the Compact style.");
            Compact_ProgressColor = config.Bind("Style - Colors", "Compact_ProgressColor", Color.white, "The color of the progress text in the Compact style.");
            Compact_PanelWidth = config.Bind("Style - Compact", "Compact Style - Panel Width", 90, "The width of the notification panel in pixels for the Compact style.");
            Compact_PanelHeight = config.Bind("Style - Compact", "Compact Style - Panel Height", 90, "The height of the notification panel in pixels for the Compact style.");
        }
    }
}
