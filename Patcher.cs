using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Hotswap.Configuration;
using NitroSharp.Formats;
using NitroSharp.Formats.ROM;
using NitroSharp.IO;
using YamlDotNet.Core;

namespace Hotswap
{
    public class Patcher
    {
        private BaseROMConfiguration BaseROMConfig { get; }
        private ProjectConfiguration ProjectConfig { get; }
        private ROM BaseROM;
        
        public Patcher(string BaseROMConfigPath, string ProjectConfigPath)
        {
             BaseROMConfig = new BaseROMConfiguration(BaseROMConfigPath);
             ProjectConfig = new ProjectConfiguration(ProjectConfigPath);
             BaseROM = new ROM(BaseROMConfig.GetROMPath(ProjectConfig.Project.BaseROMCode));
             PatchROMSettings();
             PatchROMFileSystem();
             BaseROM.Serialize($"{ProjectConfig.Project.ProjectName}.nds");
        }

        private void PatchROMSettings()
        {
            BaseROM.Header.Title = ProjectConfig.Project.ProjectGameTitle;
            BaseROM.Header.GameCode = ProjectConfig.Project.ProjectROMCode;
        }
        
        private void PatchROMFileSystem()
        {
            if (Directory.Exists(ProjectConfig.Project.NARCsPath))
            {
                List<string> NARCs = new List<string>();
                RecursiveDepthSearch(ProjectConfig.Project.NARCsPath, 0, 3, ref NARCs);
                foreach (string NARC in NARCs)
                {
                    NitroFile OriginalFile = GetFileFromOriginROM($"/a\\{Path.GetRelativePath(ProjectConfig.Project.NARCsPath, NARC)}");
                    NARC NARCFile = new NARC(OriginalFile.FileData);
                    foreach (var SrcFile in Directory.GetFiles(NARC))
                    {
                        int Index = int.Parse(Path.GetFileNameWithoutExtension(SrcFile).Split('_')[1]);
                        if (Index >= NARCFile.FAT.Entries.Count) 
                            NARCFile.FAT.Entries.Add(new FATB_Entry(File.ReadAllBytes(SrcFile)));
                        else if (Index < 0)
                            throw new Exception("Invalid file index.");
                        else
                            NARCFile.FAT.Entries[Index].Buffer = File.ReadAllBytes(SrcFile);
                    }
                    OriginalFile.FileData = NARCFile.Serialize();
                }
            }
            
            if (Directory.Exists(ProjectConfig.Project.ROMFileSystemPath))
                foreach (string File in Directory.EnumerateFiles(ProjectConfig.Project.ROMFileSystemPath, "*", SearchOption.AllDirectories))
                    PatchFile(File, Path.GetRelativePath(ProjectConfig.Project.ROMFileSystemPath, File));
        }

        public NitroFile GetFileFromOriginROM(string FilePath) => NitroDirectory.SearchDirectoryForFile(BaseROM.Root, FilePath, "/");
        
        public void PatchFile(string Path, string ROMPath)
        {
            NitroFile Foo = GetFileFromOriginROM($"/{ROMPath}");
            if (Foo != null)
                Foo.FileData = File.ReadAllBytes(Path);
        }

        private void RecursiveDepthSearch(string Parent, int ActualDepth, int TargetDepth, ref List<string> Paths)
        {
            if (ActualDepth == TargetDepth)
            {
                Paths.Add(Parent);
                return;
            }

            if (ActualDepth > TargetDepth)
                return;

            foreach (string Directory in Directory.GetDirectories(Parent))
                RecursiveDepthSearch(Directory, ActualDepth + 1, TargetDepth, ref Paths);
        }
    }
}