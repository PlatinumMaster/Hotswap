using System.CodeDom.Compiler;
using System.IO;

namespace Hotswap.Configuration
{
    public class Generator
    {
        public Generator(string ProjectName, string ProjectPath, string BaseROM)
        {
            using var Text = new IndentedTextWriter(new StreamWriter(Path.Combine(ProjectPath, $"{ProjectName}.yml")));
            Text.WriteLine($"{ProjectName.Replace(" ", "")}:");
            Text.Indent = 1;
            Text.WriteLine($"- BaseROMCode: {BaseROM}");
            Text.WriteLine("- ExecutableFileSystemPath: exefs");
            Text.WriteLine("- ROMFileSystemPath: nitrofs");
            Text.WriteLine("- ProjectROMCode: TEST");
            Text.WriteLine("- ProjectGameTitle: TESTTESTTEST");
            Text.WriteLine("- OverlayModulePath: overlays");
            Text.WriteLine("- NARCsPath: narcs");
            Text.Close();
        }
    }
}