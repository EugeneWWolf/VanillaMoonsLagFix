using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace VanillaMoonsLagFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class VanillaMoonsLagFix : BaseUnityPlugin
    {
        public static VanillaMoonsLagFix Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static ConfigEntry<string>? EnablePatchOnFollowingMoons { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            BindConfigs();

            if (EnablePatchOnFollowingMoons != null)
            {
                MoonsToBePatched.GetEnabledMoonsFromConfigString(EnablePatchOnFollowingMoons.Value);
                Logger.LogDebug("Successfully parsed EnabledPatchOnFollowingMoons.");
            }

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        public void BindConfigs()
        {
            EnablePatchOnFollowingMoons = Config.Bind("MOONS", "Enable", "Experimentation, Assurance, Vow, March, Adamance, Rend, Dine, Offense, Titan, Artifice, Embrion", string.Format("Enable patch on specified moons. Moon names are case-insensitive.\nAvailable options: {0}.", string.Join(", ", MoonsToBePatched.AllMoons_nameToId.Keys)));
            Logger.LogDebug("Successfully binded EnabledPatchOnFollowingMoons from config!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
