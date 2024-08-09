using HarmonyLib;
using UnityEngine;
using System.Text.RegularExpressions;

namespace VanillaMoonsLagFix.Patches
{
    [HarmonyPatch]
    public class RendLagFix
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevelWait))]
        [HarmonyPostfix]
        public static void LoadNewLevelWaitRendPatch(RoundManager __instance)
        {
            const int rendID = 6;

            if (__instance.currentLevel.levelID == rendID)
            {
                bool removedWindTriggers = DineTriggerRemover.RendWindTriggerRemover();

                if (removedWindTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo("Successfully disabled wind triggers on Rend.");
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError("Wind triggers on Rend seem to be unloaded, can't modify them.");
                }

                bool removedReverbTriggers = DineTriggerRemover.RendAudioReverbTriggerRemover();

                if (removedReverbTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo("Successfully disabled reverb triggers on Rend.");
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError("Reverb triggers on Rend seem to be unloaded, can't modify them.");
                }
            }
        }

        class DineTriggerRemover
        {
            internal static bool RendWindTriggerRemover()
            {
                Transform environment = GameObject.Find("/Environment").transform;
                Transform windTriggers = environment.Find("ReverbTriggers (1)/WindTriggers");

                if (windTriggers == null)
                {
                    VanillaMoonsLagFix.Logger.LogDebug("Can't find any wind triggers, probably Rend is unloaded.");

                    return false;
                }
                else
                {
                    windTriggers.gameObject.SetActive(false);

                    VanillaMoonsLagFix.Logger.LogDebug("Set " + windTriggers.gameObject.name + " activeSelf to false!");

                    return true;
                }
            }

            internal static bool RendAudioReverbTriggerRemover()
            {
                AudioReverbTrigger[] reverbTriggers = GameObject.FindObjectsByType<AudioReverbTrigger>(FindObjectsSortMode.None);

                if (reverbTriggers == null)
                {
                    VanillaMoonsLagFix.Logger.LogDebug("Can't find any reverb triggers, probably Rend is unloaded.");

                    return false;
                }
                else
                {
                    int disabledTriggersCount = 0;

                    for (int i = 0; i < reverbTriggers.Length; i++)
                    {
                        string pattern = "(LeavingShip|FallOffShip).*";

                        if (Regex.IsMatch(reverbTriggers[i].name, pattern))
                        {
                            reverbTriggers[i].gameObject.SetActive(false);

                            disabledTriggersCount++;

                            VanillaMoonsLagFix.Logger.LogDebug("Found AudioReverbTrigger object: " + reverbTriggers[i].gameObject.name);
                        }
                    }

                    VanillaMoonsLagFix.Logger.LogDebug("Set " + disabledTriggersCount + " reverb triggers' activeSelf to false!");

                    return true;
                }
            }
        }
    }
}