using System;
using System.Collections.Generic;

namespace VanillaMoonsLagFix
{
    /*
     * This class stores a list of vanilla moons, along with their corresponding IDs,
     * and a list of enabled moons specified by the mod's configuration. 
     */
    public static class MoonsToBePatched
    {
        public static readonly IDictionary<int, string> AllMoons_idToName = new Dictionary<int, string>()
        {
            { 0,   "experimentation" },
            { 1,   "assurance" },
            { 2,   "vow" },
            { 4,   "march" },
            { 5,   "adamance" },
            { 6,   "rend" },
            { 7,   "dine" },
            { 8,   "offense" },
            { 9,   "titan" },
            { 10 , "artifice" },
            { 12 , "embrion" }
        };

        public static readonly IDictionary<string, int> AllMoons_nameToId = new Dictionary<string, int>()
        {
            { "experimentation", 0 },
            { "assurance",       1 },
            { "vow",             2 },
            { "march",           4 },
            { "adamance",        5 },
            { "rend",            6 },
            { "dine",            7 },
            { "offense",         8 },
            { "titan",           9 },
            { "artifice",       10 },
            { "embrion",        12 }
        };

        public static int[] enabledMoonsIds = new int[0];

        private static bool parsedConfig = false;

        private static int[] AddElementToArray(int[] array, int element)
        {
            int[] newArray = new int[array.Length + 1];

            Array.Copy(array, newArray, array.Length);

            newArray[newArray.Length - 1] = element;

            return newArray;
        }

        public static void GetEnabledMoonsFromConfigString(string enablePatchOnFollowingMoons)
        {
            if (parsedConfig)
            {
                return;
            }

            enablePatchOnFollowingMoons = enablePatchOnFollowingMoons.ToLower();

            string[] splitStrings = enablePatchOnFollowingMoons.Split(',');

            foreach (string str in splitStrings)
            {
                string trimmedMoonName = str.Trim();

                /*
                 * If a moon name from the configuration is recognized as a vanilla moon, it's ID is added to the list of enabled moons.
                 */
                if (AllMoons_nameToId.ContainsKey(trimmedMoonName))
                {
                    int idOfMoon = AllMoons_nameToId[trimmedMoonName];

                    enabledMoonsIds = AddElementToArray(enabledMoonsIds, idOfMoon);
                }
            }

            parsedConfig = true;
        }
    }
}
