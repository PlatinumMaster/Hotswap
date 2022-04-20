using System.Collections.Generic;

namespace BeaterLibrary.GameInfo {
    public class BW : AbstractGameInformation {
        public override string[] systemsText => new[] {"0", "0", "2"};
        public override string[] mapText => new[] {"0", "0", "3"};
        public override string[] maps => new[] {"0", "0", "8"};
        public override string[] matrix => new[] {"0", "0", "9"};
        public override string[] zoneHeaders => new[] {"0", "1", "2"};
        public override string[] trainerData => new[] {"0", "9", "2"};
        public override string[] trainerPokemon => new[] {"0", "9", "3"};
        public override string[] zoneEntities => new[] {"1", "2", "5"};
        public override string[] scripts => new[] {"0", "5", "7"};
        public override string[] wildEncounters => new[] {"1", "2", "6"};
        public override int[] scriptPlugins { get; }
        public override string title => "BW";
        public override int getScriptPluginsByScrId(int scriptIndex) => -1;

        public BW() {
            ImportantSystemText = new() {
                {"ItemNames", 280},
                {"PokémonNames", 70},
                {"MapNames", 89},
                {"BattleTypes", 166},
                {"DebugSoundTestNames", 174},
                {"CreditsNames", 177},
                {"Abilities", 182},
                {"TrainerDialogue", 189},
                {"TrainerNames", 190},
                {"TrainerClasses", 191},
                {"Types", 398},
                {"MoveDescriptions", 202},
                {"MoveNames", 203},
            };
        }
    }
}