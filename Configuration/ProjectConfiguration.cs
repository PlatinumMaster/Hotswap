using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Hotswap.Configuration
{
    public class ProjectConfiguration
    {
        public ProjectDefinition Project { get; private set; }
        public ProjectConfiguration(string Path)
        {
            foreach (KeyValuePair<string, List<YamlMappingNode>> ProjectPropertiesPair in new Deserializer()
                .Deserialize<Dictionary<string, List<YamlMappingNode>>>(File.ReadAllText(Path)))
            {
                Dictionary<string, string> ParsedConfig = Util.ParseConfigurationYAML(ProjectPropertiesPair.Value, 7);
                Project = new ProjectDefinition
                {
                    ProjectName = ProjectPropertiesPair.Key,
                    BaseROMCode = ParsedConfig["BaseROMCode"],
                    ExecutableFileSystemPath = ParsedConfig["ExecutableFileSystemPath"],
                    ROMFileSystemPath = ParsedConfig["ROMFileSystemPath"],
                    ProjectROMCode = ParsedConfig["ProjectROMCode"],
                    ProjectGameTitle = ParsedConfig["ProjectGameTitle"],
                    OverlayModulePath = ParsedConfig["OverlayModulePath"],
                    NARCsPath = ParsedConfig["NARCsPath"]
                };
            }
        }
    }
}