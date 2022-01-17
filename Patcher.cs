using System;
using System.Collections.Generic;
using System.IO;
using BeaterLibrary.GameInfo;
using Hotswap.Configuration;
using NitroSharp.Formats;
using NitroSharp.Formats.ROM;
using NitroSharp.IO;

namespace Hotswap {
    public class Patcher {
        public Patcher(string baseRomConfigPath, string projectConfigPath) {
            baseRomConfig = new baseROMConfiguration();
            baseRomConfig.initializePatcher(baseRomConfigPath);
            projectConfig = new ProjectConfiguration(projectConfigPath);
            if (!File.Exists(baseRomConfig.getRomPath(projectConfig.project.baseRomCode))) {
                throw new Exception($"File not found: {baseRomConfig.getRomPath(projectConfig.project.baseRomCode)}");
            }
            baseRom = new Rom(baseRomConfig.getRomPath(projectConfig.project.baseRomCode));
        }

        private baseROMConfiguration baseRomConfig { get; }
        private ProjectConfiguration projectConfig { get; }
        private Rom baseRom { get; }
        
        private void patchRomSettings() {
            baseRom.header.title = projectConfig.project.projectGameTitle;
            baseRom.header.gameCode = projectConfig.project.projectRomCode;
        }

        private void patchRomFileSystem() {
            if (Directory.Exists(projectConfig.project.narCsPath)) {
                var narCs = new List<string>();
                recursiveDepthSearch(projectConfig.project.narCsPath, 0, 3, ref narCs);
                foreach (var narc in narCs) {
                    var originalFile =
                        getFileFromOriginRom(
                            AbstractGameInformation.getGamePath(Path
                                .GetRelativePath(projectConfig.project.narCsPath, narc)
                                .Split(Path.DirectorySeparatorChar)));
                    var narcFile = new Narc(originalFile.fileData);
                    foreach (var srcFile in Directory.GetFiles(narc)) {
                        var index = int.Parse(Path.GetFileNameWithoutExtension(srcFile));
                        if (index >= narcFile.fat.entries.Count)
                            narcFile.fat.entries.Add(new FatbEntry(File.ReadAllBytes(srcFile)));
                        else if (index < 0)
                            throw new Exception("Invalid file index.");
                        else
                            narcFile.fat.entries[index].buffer = File.ReadAllBytes(srcFile);
                    }

                    originalFile.fileData = narcFile.serialize();
                }
            }

            if (Directory.Exists(projectConfig.project.romFileSystemPath))
                foreach (var file in Directory.EnumerateFiles(projectConfig.project.romFileSystemPath, "*",
                             SearchOption.AllDirectories))
                    patchFile(file, Path.GetRelativePath(projectConfig.project.romFileSystemPath, file));
        }

        private void patchExecutableFileSystem() {
            if (Directory.Exists(projectConfig.project.executableFileSystemPath)) {
                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM9.bin")))
                    baseRom.arm9Binary.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM9.bin"));

                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM7.bin")))
                    baseRom.arm7Binary.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM7.bin"));

                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM9i.bin")))
                    baseRom.arm9IBinary.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM9i.bin"));

                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM7i.bin")))
                    baseRom.arm7IBinary.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM7i.bin"));

                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM9OverlayTable.bin")))
                    baseRom.arm9OverlayTable.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath,
                            "ARM9OverlayTable.bin"));

                if (File.Exists(Path.Combine(projectConfig.project.executableFileSystemPath, "ARM7OverlayTable.bin")))
                    baseRom.arm7OverlayTable.data =
                        File.ReadAllBytes(Path.Combine(projectConfig.project.executableFileSystemPath,
                            "ARM7OverlayTable.bin"));

                patchOverlays();
            }
        }

        public NitroFile getFileFromOriginRom(string filePath) {
            return NitroDirectory.searchDirectoryForFile(baseRom.root, filePath);
        }

        public void patchFile(string path, string romPath) {
            var foo = getFileFromOriginRom($"/{romPath}");
            if (foo != null)
                foo.fileData = File.ReadAllBytes(path);
        }

        private void recursiveDepthSearch(string parent, int actualDepth, int targetDepth, ref List<string> paths) {
            if (actualDepth == targetDepth) {
                paths.Add(parent);
                return;
            }

            if (actualDepth > targetDepth)
                return;

            foreach (var directory in Directory.GetDirectories(parent))
                recursiveDepthSearch(directory, actualDepth + 1, targetDepth, ref paths);
        }

        public byte[] fetchFileFromNarc(string[] gamePath, int id) {
            var externalPath = Path.Combine(projectConfig.project.narCsPath,
                AbstractGameInformation.getSystemPath(gamePath), $"{id}.bin");
            if (File.Exists(externalPath))
                return File.ReadAllBytes(externalPath);
            return new Narc(getFileFromOriginRom(AbstractGameInformation.getGamePath(gamePath)).fileData).fat
                .entries[id].buffer;
        }

        public NitroFile fetchFileFromRomfs(string[] gamePath) {
            var originFile = getFileFromOriginRom(AbstractGameInformation.getGamePath(gamePath));
            var externalPath = Path.Combine(projectConfig.project.romFileSystemPath,
                AbstractGameInformation.getSystemPath(gamePath));
            if (File.Exists(externalPath))
                originFile.fileData = File.ReadAllBytes(externalPath);
            return originFile;
        }

        public string getFilePathToDisk(string file) {
            return Path.Combine(projectConfig.project.romFileSystemPath, file);
        }

        public string getNarcFolderPathToDisk(string file) {
            return Path.Combine(projectConfig.project.narCsPath, file);
        }

        public void saveToNarcFolder(string[] narc, int index, Action<string> methodology) {
            var narcPath = AbstractGameInformation.getSystemPath(narc);
            Directory.CreateDirectory(getNarcFolderPathToDisk(narcPath));
            methodology(Path.Combine(getNarcFolderPathToDisk(narcPath), $"{index}.bin"));
        }

        public void patchOverlays() {
            var changedIndices = new List<int>();
            if (Directory.Exists(projectConfig.project.overlayModulePath)) {
                foreach (var overlay in Directory.GetFiles(projectConfig.project.overlayModulePath)) {
                    var index = int.Parse(Path.GetFileNameWithoutExtension(overlay));
                    if (index < 0)
                        continue;
                    if (index < baseRom.arm9Overlays.Count && index >= 0)
                        baseRom.arm9Overlays[index].data = File.ReadAllBytes(overlay);
                    else if (index >= baseRom.arm9Overlays.Count)
                        baseRom.arm9Overlays.Add(new NitroOverlay {
                            data = File.ReadAllBytes(overlay)
                        });
                    changedIndices.Add(index);
                }

                baseRom.arm9OverlayTable.data = updateArm9OverlayTable(changedIndices);
            }
        }

        public byte[] updateArm9OverlayTable(List<int> changedIndices) {
            var table = baseRom.arm9OverlayTable;
            foreach (var overlay in baseRom.arm9Overlays) {
                var index = baseRom.arm9Overlays.IndexOf(overlay);
                if (changedIndices.Contains(index))
                    if (index > table.overlayTableEntries.Count) {
                        table.overlayTableEntries.Add(new NitroOverlayTableEntry {
                            // Set this up, so expansion works out later.
                            id = (uint) index,
                            ramAddress = 0x23F900,
                            ramSize = overlay.getUncompressedSize(),
                            bssSize = 0, // Figure out how to get BSS Size, shouldn't be terrible?
                            staticInitStart = 0, //  This too...
                            staticInitEnd = 0, // This three...
                            fileId = (uint) index,
                            compressedSizeAndFlag = overlay.compressionFlag | overlay.getCompressedSize()
                        });
                    }
                    else {
                        var entry = table.overlayTableEntries[index];
                        entry.ramSize = overlay.getUncompressedSize();
                        entry.compressedSizeAndFlag = overlay.compressionFlag | overlay.getCompressedSize();
                    }
            }

            return table.serialize();
        }

        public void patchAndSerialize(string outputPath) {
            patchRomSettings();
            patchRomFileSystem();
            patchExecutableFileSystem();
            baseRom.serialize(outputPath);
        }

        public int getNarcEntryCount(string[] gamePath) {
            return new Narc(getFileFromOriginRom(AbstractGameInformation.getGamePath(gamePath)).fileData).fat.entries
                .Count;
        }

        public string getGameCode() {
            return baseRom.header.gameCode;
        }
    }
}