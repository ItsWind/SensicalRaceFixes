using SensicalRaceFixes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace SensicalRaceFixes {
    public class RacialMixingBehavior : CampaignBehaviorBase {
        public static RacialMixingBehavior Instance;

        public Dictionary<CharacterObject, CharacterRacialMix> AllCharacterRacialMixes = new();

        private List<Hero> GetAllHeroes() {
            List<Hero> allHeroes = Campaign.Current.AliveHeroes.ToList();
            allHeroes.AddRange(Campaign.Current.DeadOrDisabledHeroes);
            return allHeroes;
        }

        private void CreateCharacterRaceMixRecords() {
            List<Hero> allHeroes = GetAllHeroes();

            foreach (Hero hero in allHeroes)
                CharacterRacialMix.CreateForExisting(hero);
        }

        public RacialMixingBehavior() {
            Instance = this;
        }

        public override void RegisterEvents() {
            CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, CreateCharacterRaceMixRecords);

            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, CreateCharacterRaceMixRecords);
        }

        public override void SyncData(IDataStore dataStore) {
            dataStore.SyncData("AllCharacterRacialMixes", ref AllCharacterRacialMixes);

            if (dataStore.IsLoading) {
                List<Hero> allHeroes = GetAllHeroes();

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
