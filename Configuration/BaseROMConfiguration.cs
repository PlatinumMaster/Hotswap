using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Hotswap.Configuration {
    public class baseROMConfiguration {
        private string _path;

        public baseROMConfiguration() {
            games = new List<string>();
            baseRomLookupTable = new List<GameDefinition>();
        }

        public List<string> games { get; }
        private List<GameDefinition> baseRomLookupTable { get; }

        public void initializePatcher(string path) {
            if (!File.Exists(path))
                throw new Exception(
                    "BaseROM.yml is missing. Please ensure this is in your SwissArmyKnife folder, and try again.");
            foreach (var baseRomPair in new Deserializer()
                         .Deserialize<Dictionary<string, List<YamlMappingNode>>>(File.ReadAllText(path))) {
                games.Add(baseRomPair.Key);
                var parsedConfig = Util.parseConfigurationYaml(baseRomPair.Value, 2);
                if (!parsedConfig.ContainsKey("Path") || !parsedConfig.ContainsKey("ROMCode"))
                    throw new Exception(
                        "Malformed BaseROM configuration. Please fix the BaseROM configuration and try again.");
                baseRomLookupTable.Add(new GameDefinition {
                    path = parsedConfig["Path"],
                    romCode = parsedConfig["ROMCode"]
                });
            }
        }

        public string getRomPath(string romCode) {
            try {
                return baseRomLookupTable.Find(defs => defs.romCode.Equals(romCode)).path;
            }
            catch (Exception) {
                throw new Exception(
                    $"Could not find the ROM Code \"{romCode}\" in the database. Please ensure this is a valid ROM code, then try again.");
            }
        }

        public string getROMCode(string Name) {
            try {
                return baseRomLookupTable[games.IndexOf(Name)].romCode;
            }
            catch (Exception) {
                throw new Exception(
                    $"Could not find the name \"{Name}\" in the database.");
            }
        }
    }
}