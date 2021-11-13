using System.IO;

namespace BeaterLibrary.GameInfo
{
    public class B2W2 : AbstractGameInformation
    {
        public enum ImportantSystemText
        {
            ItemNames = 64,
            PokémonNames = 90,
            MapNames = 109,
            BattleTypes = 357,
            DebugSoundTestNames = 366,
            SpecialTrainerNames = 368,
            CreditsNames = 369,
            StartMenuMysteryGift = 371,
            Abilities = 374,
            TrainerDialogue = 381,
            TrainerNames = 382,
            TrainerClasses = 383,
            Types = 398,
            MoveDescriptions = 402,
            MoveNames = 403,
            WorldTournamentTrainerNames = 409,
            Heights = 452
        }

        public override string[] SystemsText => new[] {"0", "0", "2"};
        public override string[] MapText => new[] {"0", "0", "3"};
        public override string[] ZoneHeader => new[] {"0", "1", "2"};
        public override string[] Scripts => new[] {"0", "5", "6"};
        public override string[] TrainerData => new[] {"0", "9", "1"};
        public override string[] TrainerPokemon => new[] {"0", "9", "2"};
        public override string[] ZoneEntities => new[] {"1", "2", "6"};
        
        public override int[] GetScriptPluginsByScrID(int ScriptIndex)
        {
            // TODO: Find a better way to do this.
            switch (ScriptIndex)
            {
                case 132:
                case 134:
                case 136:
                case 138:
                case 140:
                case 142:
                case 144:
                case 146:
                case 148:
                case 150:
                case 152:
                    return new int[]{50};
                case 104:
                    return new int[]{51};
                case 280:
                case 282:
                case 284:
                case 286:
                case 288:
                    return new int[]{52};
                case 762:
                    return new int[]{53};
                case 482:
                case 484:
                case 486:
                case 488:
                case 490:
                    return new int[]{54};
                case 384:
                    return new int[]{55, 56};
                case 386:
                    return new int[]{55, 57};
                case 980:
                case 982:
                    return new int[]{58, 59};
                case 956:
                case 958:
                case 960:
                case 962:
                case 964:
                case 966:
                case 968:
                case 970:
                case 972:
                case 974:
                case 984:
                case 986:
                    return new int[]{61};
                case 1132:
                case 1134:
                case 1136:
                case 1148:
                    return new int[]{62};
                case 1146:
                    return new int[]{63};
                case 1106:
                case 1116:
                case 1122:
                case 1126:
                case 1128:
                case 1158:
                case 1160:
                case 1162:
                case 1164:
                case 1166:
                    return new int[]{64};
                case 2:
                case 16:
                case 40:
                case 82:
                case 130:
                case 198:
                case 218:
                case 230:
                case 244:
                case 292:
                case 796:
                case 814:
                case 826:
                case 850:
                case 870:
                case 886:
                case 908:
                case 920:
                case 944:
                case 1204:
                    return new int[]{65};
                case 278:
                case 426:
                case 854:
                case 1208:
                    return new int[]{66};
                case 926:
                case 930:
                case 948:
                    return new int[]{67};
                case 106:
                case 1130:
                case 1228:
                    return new int[]{68};
                default:
                    return new int[]{};
            }
        }

        public override string Title => "B2W2";
    }
}