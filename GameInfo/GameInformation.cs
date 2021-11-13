using System.Collections.Generic;

namespace BeaterLibrary.GameInfo
{
    public class GameInformation
    {
        private static readonly Dictionary<string, AbstractGameInformation> GameInfoFinder = new()
        {
            ["IRDO"] = new B2W2(),
            ["IREO"] = new B2W2()
        };

        public static AbstractGameInformation GetGameConfiguration(string GameCode)
        {
            return GameInfoFinder[GameCode];
        }
    }
}