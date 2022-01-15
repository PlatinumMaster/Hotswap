namespace BeaterLibrary.GameInfo {
    public class B2W2 : AbstractGameInformation {
        public enum ImportantSystemText {
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

        public override string[] systemsText => new[] {"0", "0", "2"};
        public override string[] mapText => new[] {"0", "0", "3"};
        public override string[] zoneHeaders => new[] {"0", "1", "2"};
        public override string[] scripts => new[] {"0", "5", "6"};
        public override string[] trainerData => new[] {"0", "9", "1"};
        public override string[] trainerPokemon => new[] {"0", "9", "2"};
        public override string[] zoneEntities => new[] {"1", "2", "6"};
        public override string[] wildEncounters => new[] {"1", "2", "7"};

        public override string[] maps => new[] {"0", "0", "8"};

        public override int[] scriptPlugins => new[] {
            50, 51, 52, 53, 54, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68
        };

        public override string title => "B2W2";

        public override int getScriptPluginsByScrId(int scriptIndex) {
            // TODO: Find a better way to do this.
            switch (scriptIndex) {
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
                    return 50;
                case 104:
                    return 51;
                case 280:
                case 282:
                case 284:
                case 286:
                case 288:
                    return 52;
                case 762:
                    return 53;
                case 482:
                case 484:
                case 486:
                case 488:
                case 490:
                    return 54;
                case 384:
                    return 55;
                case 386:
                    return 55;
                case 980:
                case 982:
                    return 58;
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
                    return 61;
                case 1132:
                case 1134:
                case 1136:
                case 1148:
                    return 62;
                case 1146:
                    return 63;
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
                    return 64;
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
                    return 65;
                case 278:
                case 426:
                case 854:
                case 1208:
                    return 66;
                case 926:
                case 930:
                case 948:
                    return 67;
                case 106:
                case 1130:
                case 1228:
                    return 68;
                default:
                    return -1;
            }
        }
    }
}