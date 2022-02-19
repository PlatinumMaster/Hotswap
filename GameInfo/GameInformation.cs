using System.Collections.Generic;

namespace BeaterLibrary.GameInfo {
    public class GameInformation {
        private static readonly Dictionary<string, AbstractGameInformation> GameInfoFinder = new() {
            ["IRAO"] = new BW(),
            ["IRBO"] = new BW(),
            ["IRDO"] = new B2W2(),
            ["IREO"] = new B2W2(),
        };

        public static AbstractGameInformation getGameConfiguration(string gameCode) {
            return GameInfoFinder[gameCode];
        }
    }
}