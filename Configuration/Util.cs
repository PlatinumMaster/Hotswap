using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.RepresentationModel;

namespace Hotswap.Configuration
{
    public class Util
    {
        public static Dictionary<string, string> ParseConfigurationYAML(List<YamlMappingNode> PropsAsNodes, int ExpectedCount)
        {
            Dictionary<string, string> Properties = new Dictionary<string, string>();
            if (PropsAsNodes != null)
            {
                if (PropsAsNodes.Count != ExpectedCount)
                    throw new Exception("Bruh you tryna break my shit?");
                    
                PropsAsNodes.ForEach(Node =>
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> KeyValuePair in Node.Children)
                    {
                        if (!Properties.TryAdd(((YamlScalarNode) KeyValuePair.Key).Value, ((YamlScalarNode) KeyValuePair.Value).Value))
                            throw new Exception($"{((YamlScalarNode) KeyValuePair.Key).Value} already defined!");
                    }
                });
            }

            return Properties;
        }

        public static void Subprocess(string Program, string Args)
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = Program,
                Arguments = Args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            proc?.Start();
            proc.WaitForExit();

            string? ErrorOutput = proc.StandardError.ReadToEnd();

            if (proc.ExitCode != 0)
                throw new Exception(ErrorOutput);
        }

    }
}