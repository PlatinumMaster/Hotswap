using System;
using System.Collections.Generic;
using System.IO;
using BeaterLibrary.GameInfo;
using Hotswap.Configuration;
using NitroSharp.Formats;
using NitroSharp.Formats.ROM;
using NitroSharp.IO;

namespace Hotswap
{
    public class Patcher
    {
        public Patcher(string BaseROMConfigPath, string ProjectConfigPath)
        {
            BaseROMConfig = new BaseROMConfiguration(BaseROMConfigPath);
            ProjectConfig = new ProjectConfiguration(ProjectConfigPath);
            BaseROM = new ROM(BaseROMConfig.GetROMPath(ProjectConfig.Project.BaseROMCode));
        }

        private BaseROMConfiguration BaseROMConfig { get; }
        private ProjectConfiguration ProjectConfig { get; }
        private ROM BaseROM { get; }

        private void PatchROMSettings()
        {
            BaseROM.Header.Title = ProjectConfig.Project.ProjectGameTitle;
            BaseROM.Header.GameCode = ProjectConfig.Project.ProjectROMCode;
        }

        private void PatchROMFileSystem()
        {
            if (Directory.Exists(ProjectConfig.Project.NARCsPath))
            {
                var NARCs = new List<string>();
                RecursiveDepthSearch(ProjectConfig.Project.NARCsPath, 0, 3, ref NARCs);
                foreach (var NARC in NARCs)
                {
                    var OriginalFile =
                        GetFileFromOriginROM(
                            AbstractGameInformation.GetGamePath(Path.GetRelativePath(ProjectConfig.Project.NARCsPath,NARC).Split(Path.DirectorySeparatorChar)));
                    var NARCFile = new NARC(OriginalFile.FileData);
                    foreach (var SrcFile in Directory.GetFiles(NARC))
                    {
                        var Index = int.Parse(Path.GetFileNameWithoutExtension(SrcFile));
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
                foreach (var File in Directory.EnumerateFiles(ProjectConfig.Project.ROMFileSystemPath, "*",
                    SearchOption.AllDirectories))
                    PatchFile(File, Path.GetRelativePath(ProjectConfig.Project.ROMFileSystemPath, File));
        }

        private void PatchExecutableFileSystem()
        {
            if (Directory.Exists(ProjectConfig.Project.ExecutableFileSystemPath))
            {
                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM9.bin")))
                    BaseROM.ARM9Binary.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM9.bin"));

                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM7.bin")))
                    BaseROM.ARM7Binary.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM7.bin"));

                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM9i.bin")))
                    BaseROM.ARM9iBinary.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM9i.bin"));

                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM7i.bin")))
                    BaseROM.ARM7iBinary.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM7i.bin"));

                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM9OverlayTable.bin")))
                    BaseROM.ARM9OverlayTable.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath,
                            "ARM9OverlayTable.bin"));

                if (File.Exists(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath, "ARM7OverlayTable.bin")))
                    BaseROM.ARM7OverlayTable.Data =
                        File.ReadAllBytes(Path.Combine(ProjectConfig.Project.ExecutableFileSystemPath,
                            "ARM7OverlayTable.bin"));

                PatchOverlays();
            }
        }

        public NitroFile GetFileFromOriginROM(string FilePath)
        {
            return NitroDirectory.SearchDirectoryForFile(BaseROM.Root, FilePath);
        }

        public void PatchFile(string Path, string ROMPath)
        {
            var Foo = GetFileFromOriginROM($"/{ROMPath}");
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

            foreach (var Directory in Directory.GetDirectories(Parent))
                RecursiveDepthSearch(Directory, ActualDepth + 1, TargetDepth, ref Paths);
        }

        public byte[] FetchFileFromNARC(string[] GamePath, int ID)
        {
            var ExternalPath = Path.Combine(ProjectConfig.Project.NARCsPath, AbstractGameInformation.GetSystemPath(GamePath), $"{ID}.bin");
            if (File.Exists(ExternalPath))
                return File.ReadAllBytes(ExternalPath);
            return new NARC(GetFileFromOriginROM(AbstractGameInformation.GetGamePath(GamePath)).FileData).FAT.Entries[ID].Buffer;
        }

        public NitroFile FetchFileFromROMFS(string[] GamePath)
        {
            var OriginFile = GetFileFromOriginROM(AbstractGameInformation.GetGamePath(GamePath));
            var ExternalPath = Path.Combine(ProjectConfig.Project.ROMFileSystemPath, AbstractGameInformation.GetSystemPath(GamePath));
            if (File.Exists(ExternalPath))
                OriginFile.FileData = File.ReadAllBytes(ExternalPath);
            return OriginFile;
        }

        public string GetFilePathToDisk(string File)
        {
            return Path.Combine(ProjectConfig.Project.ROMFileSystemPath, File);
        }

        public string GetNARCFolderPathToDisk(string File)
        {
            return Path.Combine(ProjectConfig.Project.NARCsPath, File);
        }

        public void SaveToNARCFolder(string[] NARC, int Index, Action<string> Methodology)
        {
            string NARCPath = AbstractGameInformation.GetSystemPath(NARC);
            Directory.CreateDirectory(GetNARCFolderPathToDisk(NARCPath));
            Methodology(Path.Combine(GetNARCFolderPathToDisk(NARCPath), $"{Index}.bin"));
        }

        public void PatchOverlays()
        {
            var ChangedIndices = new List<int>();
            if (Directory.Exists(ProjectConfig.Project.OverlayModulePath))
            {
                foreach (var Overlay in Directory.GetFiles(ProjectConfig.Project.OverlayModulePath))
                {
                    var Index = int.Parse(Path.GetFileNameWithoutExtension(Overlay));
                    if (Index < 0)
                        continue;
                    if (Index < BaseROM.ARM9Overlays.Count && Index >= 0)
                        BaseROM.ARM9Overlays[Index].Data = File.ReadAllBytes(Overlay);
                    else if (Index >= BaseROM.ARM9Overlays.Count)
                        BaseROM.ARM9Overlays.Add(new NitroOverlay
                        {
                            Data = File.ReadAllBytes(Overlay)
                        });
                    ChangedIndices.Add(Index);
                }

                BaseROM.ARM9OverlayTable.Data = UpdateARM9OverlayTable(ChangedIndices);
            }
        }

        public byte[] UpdateARM9OverlayTable(List<int> ChangedIndices)
        {
            var Table = BaseROM.ARM9OverlayTable;
            foreach (var Overlay in BaseROM.ARM9Overlays)
            {
                var Index = BaseROM.ARM9Overlays.IndexOf(Overlay);
                if (ChangedIndices.Contains(Index))
                    if (Index > Table.OverlayTableEntries.Count)
                    {
                        Table.OverlayTableEntries.Add(new NitroOverlayTableEntry
                        {
                            // Set this up, so expansion works out later.
                            ID = (uint) Index,
                            RAMAddress = 0x23F900,
                            RAMSize = Overlay.GetUncompressedSize(),
                            BSSSize = 0, // Figure out how to get BSS Size, shouldn't be terrible?
                            StaticInitStart = 0, //  This too...
                            StaticInitEnd = 0, // This three...
                            FileID = (uint) Index,
                            CompressedSizeAndFlag = Overlay.CompressionFlag | Overlay.GetCompressedSize()
                        });
                    }
                    else
                    {
                        var Entry = Table.OverlayTableEntries[Index];
                        Entry.RAMSize = Overlay.GetUncompressedSize();
                        Entry.CompressedSizeAndFlag = Overlay.CompressionFlag | Overlay.GetCompressedSize();
                    }
            }

            return Table.Serialize();
        }

        public void PatchAndSerialize(string OutputPath)
        {
            PatchROMSettings();
            PatchROMFileSystem();
            PatchExecutableFileSystem();
            BaseROM.Serialize(OutputPath);
        }

        public int GetNARCEntryCount(string[] GamePath)
        {
            return new NARC(GetFileFromOriginROM(AbstractGameInformation.GetGamePath(GamePath)).FileData).FAT.Entries.Count;
        }

        public string GetGameCode()
        {
            return BaseROM.Header.GameCode;
        }
    }
}