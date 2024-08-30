using HarmonyLib;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VanillaMoonsLagFix.Patches
{
    [HarmonyPatch]
    public class LagFixPatcher
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SpawnOutsideHazards))]
        [HarmonyPostfix]
        public static void PatchesExecuter(RoundManager __instance)
        {
            int currentLevelID = __instance.currentLevel.levelID;

            bool needsToBePatched = MoonsToBePatched.enabledMoonsIds.Contains(currentLevelID);

            if (needsToBePatched)
            {
                string moonName = MoonsToBePatched.AllMoons_idToName[currentLevelID];

                bool removedWindTriggers = TriggerRemover.RemoveWindTriggers();

                if (removedWindTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo(String.Format("Successfully disabled wind triggers on {0}.", moonName));
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError(String.Format("Wind triggers on {0} seem to be unloaded, can't modify them.", moonName));
                }

                bool removedReverbTriggers = TriggerRemover.ManageAudioReverbTriggers(false);

                if (removedReverbTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo(String.Format("Successfully disabled reverb triggers on {0}.", moonName));
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError(String.Format("Reverb triggers on {0} seem to be unloaded, can't modify them.", moonName));
                }
            }
            else
            {
                /*
                 * The decision to re-enable these scene triggers is based on the fact that some triggers are permanently disabled until manually re-enabled.
                 * I suspected this might cause issues, so I re-enabled them when players arrive at the moon, where the patch is disabled.
                 */
                bool enabledReverbTriggers = TriggerRemover.ManageAudioReverbTriggers(true);

                if (enabledReverbTriggers)
                {
                    VanillaMoonsLagFix.Logger.LogInfo(String.Format("Successfully re-enabled reverb triggers on {0} (patch is disabled here).", __instance.currentLevel.PlanetName));
                }
                else
                {
                    VanillaMoonsLagFix.Logger.LogError(String.Format("Reverb triggers on {0} seem to be unloaded, can't re-enable them.", __instance.currentLevel.PlanetName));
                }
            }
        }

        class TriggerRemover
        {
            internal static bool RemoveWindTriggers()
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

            /*
             * state = true -> re-enable disabled AudioReverbTriggers.
             * state = false -> disable AudioReverbTriggers.
             */
            internal static bool ManageAudioReverbTriggers(bool state)
            {
                // FindObjectsOfTypeAll is used to include inactive objects of type AudioReverbTrigger in the search results.
                AudioReverbTrigger[] reverbTriggers = Resources.FindObjectsOfTypeAll<AudioReverbTrigger>(); ;

                if (reverbTriggers == null)
                {
                    return false;
                }
                else
                {
                    string pattern = "(LeavingShip|FallOffShip).*";

                    int changedTriggersCount = 0;

                    for (int i = 0; i < reverbTriggers.Length; i++)
                    {
                        if (Regex.IsMatch(reverbTriggers[i].name, pattern))
                        {
                            reverbTriggers[i].gameObject.SetActive(state);

                            changedTriggersCount++;

                            VanillaMoonsLagFix.Logger.LogDebug(String.Format("Found AudioReverbTrigger object: {0}", reverbTriggers[i].gameObject.name));
                        }
                    }

                    VanillaMoonsLagFix.Logger.LogDebug(String.Format("Changed state of {0} ReverbTriggers activeSelf! (they are now enabled: {1})", changedTriggersCount, state));

                    return true;
                }
            }
        }
    }
}
