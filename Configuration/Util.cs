using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.RepresentationModel;

namespace Hotswap.Configuration {
    public class Util {
        public static Dictionary<string, string> parseConfigurationYaml(List<YamlMappingNode> propsAsNodes,
            int expectedCount) {
            var properties = new Dictionary<string, string>();
            if (propsAsNodes != null) {
                if (propsAsNodes.Count != expectedCount)
                    throw new Exception("Bruh you tryna break my shit?");

                propsAsNodes.ForEach(node => {
                    foreach (var keyValuePair in node.Children)
                        if (!properties.TryAdd(((YamlScalarNode) keyValuePair.Key).Value,
                                ((YamlScalarNode) keyValuePair.Value).Value))
                            throw new Exception($"{((YamlScalarNode) keyValuePair.Key).Value} already defined!");
                });
            }

            return properties;
        }

        public static void subprocess(string program, string args) {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo {
                FileName = program,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            proc?.Start();
            proc.WaitForExit();

            var errorOutput = proc.StandardError.ReadToEnd();

            if (proc.ExitCode != 0)
                throw new Exception(errorOutput);
        }
    }
}