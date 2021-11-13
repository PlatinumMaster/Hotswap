using System.IO;

namespace BeaterLibrary.GameInfo
{
    public abstract class AbstractGameInformation
    {
        public abstract string[] SystemsText { get; }
        public abstract string[] MapText { get; }
        public abstract string[] ZoneHeader { get; }
        public abstract string[] TrainerData { get; }
        public abstract string[] TrainerPokemon { get; }
        public abstract string[] ZoneEntities { get; }
        public abstract string[] Scripts { get; }
        public abstract int[] GetScriptPluginsByScrID(int ScriptIndex);
        public abstract string Title { get; }
        public static string GetSystemPath(string[] Data) => Path.Combine(Data);
        public static string GetGamePath(string[] Data) => "/a/" + string.Join("/", Data);
    }
}