using BepInEx.Configuration;

namespace IsoBadges
{
    // Enum to define the different UI styles
    public enum AchievementUIStyle
    {
        Default,
        Compact
    }

    // Static class to hold all config entries
    public static class ConfigManager
    {
        // General Settings
        public static ConfigEntry<bool> ShowProgressPopups { get; private set; }
        public static ConfigEntry<AchievementUIStyle> UIStyle { get; private set; }

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

            // Default Style Section
            Default_PanelWidth = config.Bind("Style - Default", "Default Style - Panel Width", 260, "The width of the notification panel in pixels for the Default style.");
            Default_PanelHeight = config.Bind("Style - Default", "Default Style - Panel Height", 70, "The height of the notification panel in pixels for the Default style.");
            Default_IconSize = config.Bind("Style - Default", "Default Style - Icon Size", 50, "The size (width and height) of the badge icon in pixels for the Default style.");
            Default_TitleFontSize = config.Bind("Style - Default", "Default Style - Title Font Size", 16, "The font size for the achievement title for the Default style.");
            Default_DescriptionFontSize = config.Bind("Style - Default", "Default Style - Description Font Size", 14, "The font size for the achievement description and progress for the Default style.");

            // Compact Style Section
            Compact_PanelWidth = config.Bind("Style - Compact", "Compact Style - Panel Width", 90, "The width of the notification panel in pixels for the Compact style.");
            Compact_PanelHeight = config.Bind("Style - Compact", "Compact Style - Panel Height", 90, "The height of the notification panel in pixels for the Compact style.");
            Compact_IconSize = config.Bind("Style - Compact", "Compact Style - Icon Size", 60, "The size (width and height) of the badge icon in pixels for the Compact style.");
            Compact_ProgressFontSize = config.Bind("Style - Compact", "Compact Style - Progress Font Size", 14, "The font size for the progress/unlocked text for the Compact style.");
        }
    }
}
