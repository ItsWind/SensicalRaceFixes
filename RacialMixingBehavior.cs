using SensicalRaceFixes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace SensicalRaceFixes {
    public class RacialMixingBehavior : CampaignBehaviorBase {
        public static RacialMixingBehavior Instance;

        public Dictionary<CharacterObject, CharacterRacialMix> AllCharacterRacialMixes = new();

        public RacialMixingBehavior() {
            Instance = this;
        }

        public override void RegisterEvents() {
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, () => {
                List<Hero> allHeroes = Campaign.Current.AliveHeroes.ToList();
                allHeroes.AddRange(Campaign.Current.DeadOrDisabledHeroes);

                foreach (Hero hero in allHeroes)
                    CharacterRacialMix.CreateForExisting(hero);
            });
        }

        public override void SyncData(IDataStore dataStore) {
            dataStore.SyncData("AllCharacterRacialMixes", ref AllCharacterRacialMixes);

            if (dataStore.IsLoading) {
                List<Hero> allHeroes = Campaign.Current.AliveHeroes.ToList();
                allHeroes.AddRange(Campaign.Current.DeadOrDisabledHeroes);

                foreach (KeyValuePair<CharacterObject, CharacterRacialMix> pair in AllCharacterRacialMixes.ToList()) {
                    // Remove characters that are no longer in campaign at all.
                    if (!allHeroes.Contains(pair.Key.HeroObject)) {
                        AllCharacterRacialMixes.Remove(pair.Key);
                        continue;
                    }

                    // Set each character race on load.
                    pair.Value.SetCharacterRace();
                }
            }
        }
    }

    public class CustomSaveDefiner : SaveableTypeDefiner {
        public CustomSaveDefiner() : base(421595386) { }

        protected override void DefineClassTypes() {
            AddClassDefinition(typeof(CharacterRacialMix), 1);
        }

        protected override void DefineContainerDefinitions() {
            ConstructContainerDefinition(typeof(Dictionary<CharacterObject, CharacterRacialMix>));
            ConstructContainerDefinition(typeof(Dictionary<int, double>));
        }
    }
}
