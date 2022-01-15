using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Hotswap.Configuration {
    public class ProjectConfiguration {
        public ProjectConfiguration(string configPath) {
            foreach (var projectPropertiesPair in new Deserializer()
                         .Deserialize<Dictionary<string, List<YamlMappingNode>>>(File.ReadAllText(configPath))) {
                var parsedConfig = Util.parseConfigurationYaml(projectPropertiesPair.Value, 7);
                project = new ProjectDefinition {
                    projectName = projectPropertiesPair.Key,
                    baseRomCode = parsedConfig["BaseROMCode"],
                    executableFileSystemPath = Path.Combine(Directory.GetParent(configPath).FullName,
                        parsedConfig["ExecutableFileSystemPath"]),
                    romFileSystemPath = Path.Combine(Directory.GetParent(configPath).FullName,
                        parsedConfig["ROMFileSystemPath"]),
                    projectRomCode = parsedConfig["ProjectROMCode"],
                    projectGameTitle = parsedConfig["ProjectGameTitle"],
                    overlayModulePath = Path.Combine(Directory.GetParent(configPath).FullName,
                        parsedConfig["OverlayModulePath"]),
                    narCsPath = Path.Combine(Directory.GetParent(configPath).FullName, parsedConfig["NARCsPath"])
                };
            }
        }

        public ProjectDefinition project { get; }
    }
}