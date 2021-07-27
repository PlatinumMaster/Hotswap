using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core.Tokens;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Hotswap.Configuration
{
    public class BaseROMConfiguration
    {
        public List<GameDefinition> BaseROMLookupTable { get; }
        public BaseROMConfiguration(string Path)
        {
            BaseROMLookupTable = new List<GameDefinition>();
            foreach (KeyValuePair<string, List<YamlMappingNode>> BaseROMPair in new Deserializer()
                .Deserialize<Dictionary<string, List<YamlMappingNode>>>(File.ReadAllText(Path)))
            {
                Dictionary<string, string> ParsedConfig = Util.ParseConfigurationYAML(BaseROMPair.Value, 2);
                if (!ParsedConfig.ContainsKey("Path") || !ParsedConfig.ContainsKey("ROMCode"))
                    throw new Exception("Lmao got em");
                BaseROMLookupTable.Add(new GameDefinition
                {
                    Path = ParsedConfig["Path"],
                    ROMCode = ParsedConfig["ROMCode"],
                });
            }
        }

        public string GetROMPath(string ROMCode)
        {
            if (ROMCode.Length != 4)
                throw new Exception("Invalid ROM Code. They must be of length 4.");

            try
            {
                return BaseROMLookupTable.Find(Defs => Defs.ROMCode.Equals(ROMCode)).Path;
            }
            catch (Exception)
            {
                throw new Exception($"Could not find the ROM Code \"{ROMCode}\" in the database. Make sure it is there, and try again.");
            }
        }
    }
}