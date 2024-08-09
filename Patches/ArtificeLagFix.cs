﻿using HarmonyLib;
using UnityEngine;
using System.Text.RegularExpressions;

namespace VanillaMoonsLagFix.Patches
{
    [HarmonyPatch]
    public class ArtificeLagFix
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevelWait))]
        [HarmonyPostfix]
        public static void LoadNewLevelWaitArtificePatch(RoundManager __instance)
        {
            const int artificeID = 10;

            if (__instance.currentLevel.levelID == artificeID)
            {
                bool removedWindTriggers = ArtificeTriggerRemover.ArtificeWindTriggerRemover();

                if (removedWindTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo("Successfully disabled wind triggers on Artifice.");
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError("Wind triggers on Artifice seem to be unloaded, can't modify them.");
                }

                bool removedReverbTriggers = ArtificeTriggerRemover.ArtificeAudioReverbTriggerRemover();

                if (removedReverbTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo("Successfully disabled reverb triggers on Artifice.");
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError("Reverb triggers on Artifice seem to be unloaded, can't modify them.");
                }
            }
        }

        class ArtificeTriggerRemover
        {
            internal static bool ArtificeWindTriggerRemover()
            {
                Transform environment = GameObject.Find("/Environment").transform;
                Transform windTriggers = environment.Find("ReverbTriggers (1)/WindTriggers");

                if (windTriggers == null)
                {
                    VanillaMoonsLagFix.Logger.LogDebug("Can't find any wind triggers, probably Artifice is unloaded.");

                    return false;
                }
                else
                {
                    windTriggers.gameObject.SetActive(false);

                    VanillaMoonsLagFix.Logger.LogDebug("Set " + windTriggers.gameObject.name + " activeSelf to false!");

                    return true;
                }
            }

            internal static bool ArtificeAudioReverbTriggerRemover()
            {
                AudioReverbTrigger[] reverbTriggers = GameObject.FindObjectsByType<AudioReverbTrigger>(FindObjectsSortMode.None);

                if (reverbTriggers == null)
                {
                    VanillaMoonsLagFix.Logger.LogDebug("Can't find any reverb triggers, probably Artifice is unloaded.");

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
