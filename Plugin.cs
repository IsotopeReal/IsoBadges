using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration; // Required for BepInEx configs
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using MoreBadges;
using PEAKLib.Core;
using IsoBadges; // Using directive for your UI code

// --- Helper Enums & Classes ---
public enum Segment : byte { Beach, Tropics, Alpine, Caldera, TheKiln, Peak }
public class MapHandler { public void GoToSegment(Segment segment) { } }


// --- NEW: Static class to hold all config entries ---
public static class ConfigManager
{
    public static ConfigEntry<bool> ShowProgressPopups { get; private set; }
    public static ConfigEntry<AchievementUIStyle> UIStyle { get; private set; }

    public static void Setup(ConfigFile config)
    {
        ShowProgressPopups = config.Bind(
            "General",
            "ShowProgressPopups",
            true,
            "If true, a pop-up will appear every time progress is made on a badge. If false, it will only appear when the badge is fully unlocked.");

        UIStyle = config.Bind(
            "General",
            "UIStyle",
            AchievementUIStyle.Default,
            "The visual style of the achievement pop-up. 'Default' shows the icon, title, and description. 'Compact' shows only the icon and title.");
    }
}


[BepInPlugin("com.isopod.isobadges", "IsoBadges", "1.0.0")]
[BepInDependency("com.snosz.morebadges", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(CorePlugin.Id, BepInDependency.DependencyFlags.HardDependency)]
public class IsoBadgesPlugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony("com.isopod.isobadges");
    public static ManualLogSource Log;

    private Texture2D coconutBadgeIcon, berryBadgeIcon, bananaBadgeIcon, marshmallowBadgeIcon, lollipopBadgeIcon, volcanoBadgeIcon, campfireBadgeIcon;

    public static class BadgeNames
    {
        public const string CoconutEnjoyer = "Badge_IsoBadges_CoconutEnjoyer";
        public const string BerryFiend = "Badge_IsoBadges_BerryFiend";
        public const string BananaCollector = "Badge_IsoBadges_BananaCollector";
        public const string MarshmallowMuncher = "Badge_IsoBadges_MarshmallowMuncher";
        public const string LollipopLover = "Badge_IsoBadges_LollipopLover";
        public const string VolcanoConqueror = "Badge_IsoBadges_VolcanoConqueror";
        public const string Firestarter = "Badge_IsoBadges_Firestarter";
    }

    private void Awake()
    {
        Log = Logger;

        // --- Setup the configuration ---
        ConfigManager.Setup(Config);

        CustomAchievementManager.Initialize(this);
        this.LoadBundleWithName("isobadges.peakbundle", OnBundleLoaded);
        GlobalEvents.OnItemConsumed += OnItemConsumed;
        harmony.PatchAll();
        Log.LogInfo("IsoBadges mod has initialized.");
    }

    private void OnBundleLoaded(PeakBundle bundle)
    {
        try
        {
            Log.LogInfo("Loading badge textures from asset bundle...");
            coconutBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_coconut");
            berryBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_berry");
            bananaBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_banana");
            marshmallowBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_marshmallow");
            lollipopBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_lollipop");
            volcanoBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_volcano");
            campfireBadgeIcon = bundle.LoadAsset<Texture2D>("isobadges_campfire");
            Log.LogInfo("All badge textures loaded. Registering badges.");
            CreateAndRegisterBadges();
        }
        catch (Exception e) { Log.LogError($"An error occurred while loading assets: {e}"); }
    }

    private void CreateAndRegisterBadges()
    {
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.CoconutEnjoyer, "Coconut Enjoyer", "Eat 100 coconuts.", null, null, coconutBadgeIcon, 3, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.BerryFiend, "Berry Fiend", "Eat 100 berries.", null, null, berryBadgeIcon, 2, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.BananaCollector, "Banana Collector", "Eat 100 bananas.", null, null, bananaBadgeIcon, 2, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.MarshmallowMuncher, "Marshmallow Muncher", "Eat 200 marshmallows.", null, null, marshmallowBadgeIcon, 2, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.LollipopLover, "Lollipop Lover", "Eat 100 lollipops.", null, null, lollipopBadgeIcon, 100, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.VolcanoConqueror, "Volcano Conqueror", "Reach the volcano 50 times.", null, null, volcanoBadgeIcon, 50, false));
        CustomAchievementManager.RegisterCustomBadge(new MoreBadgesPlugin.CustomBadge(BadgeNames.Firestarter, "Firestarter", "Light a campfire 200 times.", null, null, campfireBadgeIcon, 200, false));
        Log.LogInfo("All custom badges have been created and registered.");
    }

    private void OnItemConsumed(Item item, Character character)
    {
        if (character.IsLocal && item != null)
        {
            string itemName = item.UIData.itemName.ToLowerInvariant();
            string badgeToProgress = null;

            if (itemName.Contains("coconut")) badgeToProgress = BadgeNames.CoconutEnjoyer;
            else if (itemName.Contains("berry")) badgeToProgress = BadgeNames.BerryFiend;
            else if (itemName.Contains("banana")) badgeToProgress = BadgeNames.BananaCollector;
            else if (itemName.Contains("marshmallow")) badgeToProgress = BadgeNames.MarshmallowMuncher;
            else if (itemName.Contains("lollipop")) badgeToProgress = BadgeNames.LollipopLover;

            if (badgeToProgress != null)
            {
                CustomAchievementManager.AddProgressAndShowAchievement(badgeToProgress, 1);
            }
        }
    }

    private void OnDestroy()
    {
        GlobalEvents.OnItemConsumed -= OnItemConsumed;
        harmony.UnpatchSelf();
    }
}

public static class CustomAchievementManager
{
    private static IsoBadgesPlugin pluginInstance;
    private static Dictionary<string, MoreBadgesPlugin.CustomBadge> registeredBadges = new Dictionary<string, MoreBadgesPlugin.CustomBadge>();

    public static void Initialize(IsoBadgesPlugin plugin)
    {
        pluginInstance = plugin;
    }

    public static void RegisterCustomBadge(MoreBadgesPlugin.CustomBadge badge)
    {
        if (!registeredBadges.ContainsKey(badge.name))
        {
            registeredBadges.Add(badge.name, badge);
            MoreBadgesPlugin.RegisterBadge(badge);
        }
    }

    public static void AddProgressAndShowAchievement(string badgeName, int amount)
    {
        MoreBadgesPlugin.CustomBadgeStatus? statusBefore = MoreBadgesPlugin.GetCustomBadgeStatus(badgeName);
        if (statusBefore == null || statusBefore.isUnlocked) return;

        MoreBadgesPlugin.AddProgress(badgeName, amount);

        int newProgress = MoreBadgesPlugin.GetProgress(badgeName);
        if (newProgress == -1) return;

        bool justUnlocked = newProgress >= registeredBadges[badgeName].progressRequired;

        if (ConfigManager.ShowProgressPopups.Value || justUnlocked)
        {
            if (registeredBadges.TryGetValue(badgeName, out var badge))
            {
                AchievementUIFactory.CreateOrUpdateAchievementPopup(badge, newProgress);
            }
        }
    }
}

[HarmonyPatch(typeof(Campfire), "Light_Rpc")]
public static class CampfireLightPatch
{
    static void Postfix()
    {
        CustomAchievementManager.AddProgressAndShowAchievement(IsoBadgesPlugin.BadgeNames.Firestarter, 1);
    }
}

[HarmonyPatch(typeof(MapHandler), "GoToSegment")]
public static class MapSegmentPatch
{
    static void Postfix(Segment segment)
    {
        if (segment == Segment.TheKiln)
        {
            CustomAchievementManager.AddProgressAndShowAchievement(IsoBadgesPlugin.BadgeNames.VolcanoConqueror, 1);
        }
    }
}
