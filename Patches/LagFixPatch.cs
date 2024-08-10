using HarmonyLib;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace VanillaMoonsLagFix.Patches
{
    enum MoonsIDs
    {
        marchID = 4,
        rendID = 6,
        dineID = 7,
        artificeID = 10
    }

    [HarmonyPatch]
    public class LagFixPatcher
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevelWait))]
        [HarmonyPostfix]
        public static void LoadNewLevelWaitArtificePatch(RoundManager __instance)
        {
            bool needsToBePatched = Enum.IsDefined(typeof(MoonsIDs), __instance.currentLevel.levelID);

            if (needsToBePatched)
            {
                bool removedWindTriggers = TriggerRemover.WindTriggerRemover();

                string levelName = __instance.currentLevel.name;

                if (removedWindTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo(String.Format("Successfully disabled wind triggers on {0}.", levelName));
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError(String.Format("Wind triggers on {0} seem to be unloaded, can't modify them.", levelName));
                }

                bool removedReverbTriggers = TriggerRemover.AudioReverbTriggerRemover();

                if (removedReverbTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo(String.Format("Successfully disabled reverb triggers on {0}.", levelName));
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError(String.Format("Reverb triggers on {0} seem to be unloaded, can't modify them.", levelName));
                }
            }
        }

        class TriggerRemover
        {
            internal static bool WindTriggerRemover()
            {
                Transform environment = GameObject.Find("/Environment").transform;
                Transform windTriggers = environment.Find("ReverbTriggers (1)/WindTriggers");
                

                if (windTriggers == null)
                {
                    return false;
                }
                else
                {
                    string pattern = "Cube.*";

                    int disabledTriggersCount = 0;

                    for (int i = 0; i < windTriggers.childCount; i++)
                    {
                        Transform childObject = windTriggers.GetChild(i);

                        if (Regex.IsMatch(childObject.name, pattern))
                        {
                            childObject.gameObject.SetActive(false);

                            disabledTriggersCount++;

                            VanillaMoonsLagFix.Logger.LogDebug(String.Format("Found WindTrigger object: {0}", childObject.name));
                        }
                    }

                    VanillaMoonsLagFix.Logger.LogDebug(String.Format("Set {0} of WindTrigger activeSelf to false!", disabledTriggersCount));

                    return true;
                }
            }

            internal static bool AudioReverbTriggerRemover()
            {
                AudioReverbTrigger[] reverbTriggers = GameObject.FindObjectsByType<AudioReverbTrigger>(FindObjectsSortMode.None);

                if (reverbTriggers == null)
                {
                    return false;
                }
                else
                {
                    string pattern = "(LeavingShip|FallOffShip).*";

                    int disabledTriggersCount = 0;

                    for (int i = 0; i < reverbTriggers.Length; i++)
                    {
                        if (Regex.IsMatch(reverbTriggers[i].name, pattern))
                        {
                            reverbTriggers[i].gameObject.SetActive(false);

                            disabledTriggersCount++;

                            VanillaMoonsLagFix.Logger.LogDebug(String.Format("Found AudioReverbTrigger object: {0}", reverbTriggers[i].gameObject.name));
                        }
                    }

                    VanillaMoonsLagFix.Logger.LogDebug(String.Format("Set {0} of ReverbTriggers activeSelf to false!", disabledTriggersCount));

                    return true;
                }
            }
        }
    }
}
