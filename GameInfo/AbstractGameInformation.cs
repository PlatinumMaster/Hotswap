using System.Collections.Generic;
using System.IO;

namespace BeaterLibrary.GameInfo {
    public abstract class AbstractGameInformation {
        public Dictionary<string, int> ImportantSystemText;
        public abstract string[] systemsText { get; }
        public abstract string[] mapText { get; }
        public abstract string[] zoneHeaders { get; }
        public abstract string[] trainerData { get; }
        public abstract string[] trainerPokemon { get; }
        public abstract string[] zoneEntities { get; }
        public abstract string[] scripts { get; }
        public abstract string[] wildEncounters { get; }
        public abstract int[] scriptPlugins { get; }
        public abstract string title { get; }
        public abstract string[] maps { get; }
        public abstract string[] matrix { get; }

        public abstract int getScriptPluginsByScrId(int scriptIndex);

        public static string getSystemPath(string[] data) {
            return Path.Combine(data);
        }

        public static string getGamePath(string[] data) {
            return "/a/" + string.Join("/", data);
        }
    }
}