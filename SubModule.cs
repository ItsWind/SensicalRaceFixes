using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using System;

namespace SensicalRaceFixes {
    public class SubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            new Harmony("SensicalRaceFixes").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter) {
            if (game.GameType is Campaign) {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new RacialMixingBehavior());
            }
        }
        public static void PrintToMessages(string str, float r = 255, float g = 255, float b = 255) {
            float[] newValues = { r / 255.0f, g / 255.0f, b / 255.0f };
            Color col = new(newValues[0], newValues[1], newValues[2]);
            InformationManager.DisplayMessage(new InformationMessage(str, col));
        }
    }
}