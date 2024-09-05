using SensicalRaceFixes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SensicalRaceFixes {
    public class ConsoleCommands {
        [CommandLineFunctionality.CommandLineArgumentFunction("debug_print_race_indexes", "sensicalracefixes")]
        private static string DebugPrintRaceIndexes(List<string> args) {
            string toReturn = "RACE NAMES AND INDEXES\n";
            for (int i = 0; i < FaceGen.GetRaceNames().Count(); i++) {
                string raceName = FaceGen.GetRaceNames()[i];
                toReturn += i + " - " + raceName + "\n";
            }
            return toReturn;
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_print_hero_races", "sensicalracefixes")]
        private static string DebugPrintHeroRaces(List<string> args) {
            if (args.Count < 1) {
                return "Need hero name.";
            } else {
                string heroNameGiven = args[0];
                Hero? heroToPrint = null;
                List<Hero> allHeroes = Campaign.Current.AliveHeroes.ToList();
                allHeroes.AddRange(Campaign.Current.DeadOrDisabledHeroes);

                foreach (Hero hero in allHeroes) {
                    if (hero.Name.ToString().ToLower().Replace(" ", "") == heroNameGiven.ToLower()) {
                        heroToPrint = hero;
                        break;
                    }
                }

                if (heroToPrint != null) {
                    CharacterRacialMix? data = CharacterRacialMix.GetForCharacter(heroToPrint.CharacterObject);
                    if (data == null)
                        return "No race data created for that hero yet.";

                    string toReturn = "RACE DATA FOR " + heroToPrint.Name.ToString() + "\n";
                    toReturn += "------------------------------------\n";

                    string[] raceNames = FaceGen.GetRaceNames();
                    foreach (KeyValuePair<int, double> pair in data.Races)
                        toReturn += raceNames[pair.Key] + " - " + pair.Value.ToString() + "\n";
                    return toReturn;
                } else
                    return "No hero with that name found. Try with no spaces and check for spelling.";
            }
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_change_hero_races", "sensicalracefixes")]
        private static string DebugChangeHeroRaces(List<string> args) {
            if (args.Count < 2) {
                return "Correct usage: mixedraces.debug_change_hero_races HeroNameNoSpaces 0|50 1|50 RaceIndex|RaceValue.";
            } else {
                string heroNameGiven = args[0];

                Hero? heroToPrint = null;
                List<Hero> allHeroes = Campaign.Current.AliveHeroes.ToList();
                allHeroes.AddRange(Campaign.Current.DeadOrDisabledHeroes);

                foreach (Hero hero in allHeroes) {
                    if (hero.Name.ToString().ToLower().Replace(" ", "") == heroNameGiven.ToLower()) {
                        heroToPrint = hero;
                        break;
                    }
                }

                if (heroToPrint != null) {
                    Dictionary<int, double> newRaces = new();
                    for (int i = 1; i < args.Count; i++) {
                        string arg = args[i];

                        int indexOfSep = arg.IndexOf("|");
                        if (indexOfSep == -1)
                            continue;

                        int raceIndex = -1;
                        if (!Int32.TryParse(arg.Substring(0, indexOfSep), out raceIndex))
                            return "One of your race index values cannot be converted to int.";

                        if (raceIndex >= FaceGen.GetRaceNames().Count())
                            return "That race index doesn't exist... probably.";

                        double valueToSet = -1;
                        if (!Double.TryParse(arg.Substring(indexOfSep + 1, arg.Length - (indexOfSep + 1)), out valueToSet))
                            return "One of your race float values cannot be converted to double.";

                        newRaces[raceIndex] = valueToSet;
                    }

                    CharacterRacialMix? data = CharacterRacialMix.GetForCharacter(heroToPrint.CharacterObject);
                    if (data == null)
                        return "No race data created for that hero yet.";

                    // CHANGE RACE MIX
                    data.Races = newRaces;
                    data.SetCharacterRace();

                    string toReturn = "RACE DATA FOR " + heroToPrint.Name.ToString() + "\n";
                    toReturn += "------------------------------------\n";

                    string[] raceNames = FaceGen.GetRaceNames();
                    foreach (KeyValuePair<int, double> pair in data.Races)
                        toReturn += raceNames[pair.Key] + " - " + pair.Value.ToString() + "\n";
                    return toReturn;
                } else
                    return "No hero with that name found. Try with no spaces and check for spelling.";
            }
        }
    }
}
