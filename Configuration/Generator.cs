using System.CodeDom.Compiler;
using System.IO;

namespace Hotswap.Configuration {
    public class Generator {
        public Generator(string projectName, string projectPath, string baseRom) {
            var dirPath = Path.Combine(projectPath, projectName);
            Directory.CreateDirectory(dirPath);
            using var text = new IndentedTextWriter(new StreamWriter(Path.Combine(dirPath, $"{projectName}.yml")));
            text.WriteLine($"{projectName.Replace(" ", "")}:");
            text.Indent = 1;
            text.WriteLine($"- BaseROMCode: {baseRom}");
            text.WriteLine("- ExecutableFileSystemPath: exefs");
            text.WriteLine("- ROMFileSystemPath: nitrofs");
            text.WriteLine("- ProjectROMCode: TEST");
            text.WriteLine("- ProjectGameTitle: TESTTESTTEST");
            text.WriteLine("- OverlayModulePath: overlays");
            text.WriteLine("- NARCsPath: narcs");
            text.Close();
        }
    }
}