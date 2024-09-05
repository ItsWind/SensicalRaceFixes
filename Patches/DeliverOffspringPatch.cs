using HarmonyLib;
using SensicalRaceFixes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace SensicalRaceFixes.Patches {
    [HarmonyPatch(typeof(HeroCreator), nameof(HeroCreator.DeliverOffSpring))]
    internal class DeliverOffspringPatch {
        [HarmonyPrefix]
        private static void Prefix(Hero mother, Hero father, ref (int, int) __state) {
            __state.Item1 = mother.CharacterObject.Race;
            __state.Item2 = father.CharacterObject.Race;

            mother.CharacterObject.Race = 0;
            father.CharacterObject.Race = 0;
        }

        [HarmonyPostfix]
        private static void Postfix(Hero mother, Hero father, Hero __result, ref (int, int) __state) {
            mother.CharacterObject.Race = __state.Item1;
            father.CharacterObject.Race = __state.Item2;

            CharacterRacialMix.CreateForNewborn(__result);
        }
    }
}
