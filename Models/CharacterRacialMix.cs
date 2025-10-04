using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace SensicalRaceFixes.Models {
    public class CharacterRacialMix {
        [SaveableField(1)]
        public CharacterObject Character;
        [SaveableField(2)]
        public Dictionary<int, double> Races = new();

        public CharacterRacialMix(CharacterObject character) {
            Character = character;
        }

        public string GetCharacterRaceName() {
            return FaceGen.GetRaceNames()[Character.Race];
        }

        public void SetCharacterRace() {
            int raceToSet = 0;
            try {
                raceToSet = Races.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }
            catch (InvalidOperationException) { }

            Character.Race = raceToSet;
        }

        public static CharacterRacialMix? GetForCharacter(CharacterObject character) {
            try {
                return RacialMixingBehavior.Instance.AllCharacterRacialMixes[character];
            }
            catch (KeyNotFoundException) {
                return null;
            }
        }

        internal static void CreateForExisting(Hero hero) {
            CharacterObject character = hero.CharacterObject;
            if (GetForCharacter(character) != null)
                return;

            Create(character);
        }

        internal static void CreateForNewborn(Hero hero) {
            Hero father = hero.Father;
            Hero mother = hero.Mother;

            CharacterRacialMix? fatherRacialMix = GetForCharacter(father.CharacterObject);
            CharacterRacialMix? motherRacialMix = GetForCharacter(mother.CharacterObject);

            if (fatherRacialMix == null)
                fatherRacialMix = Create(father.CharacterObject);
            if (motherRacialMix == null)
                motherRacialMix = Create(mother.CharacterObject);

            Dictionary<int, double> newbornRaces = new();

            foreach (KeyValuePair<int, double> pair in fatherRacialMix.Races) {
                double motherRaceValue = 0.0;
                if (motherRacialMix.Races.ContainsKey(pair.Key))
                    motherRaceValue = motherRacialMix.Races[pair.Key];

                double newbornRaceValue = (motherRaceValue + pair.Value) / 2.0;

                newbornRaces[pair.Key] = newbornRaceValue;
            }

            foreach (KeyValuePair<int, double> pair in motherRacialMix.Races) {
                if (newbornRaces.ContainsKey(pair.Key))
                    continue;

                newbornRaces[pair.Key] = pair.Value / 2;
            }

            Create(hero.CharacterObject, newbornRaces);
        }

        private static CharacterRacialMix Create(CharacterObject character, Dictionary<int, double>? raceMix = null) {
            if (raceMix == null)
                raceMix = new Dictionary<int, double> {
                    { character.Race, 100.0 }
                };

            CharacterRacialMix racialMix = new CharacterRacialMix(character);
            racialMix.Races = raceMix;

            RacialMixingBehavior.Instance.AllCharacterRacialMixes[character] = racialMix;

            racialMix.SetCharacterRace();

            return racialMix;
        }
    }
}
