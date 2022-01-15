namespace Hotswap.Configuration {
    public class ProjectDefinition {
        // Attributes that are customizable by the user.
        public string projectName { get; set; }
        public string projectRomCode { get; set; }
        public string projectGameTitle { get; set; }

        // General attributes for Hotswap.
        public string romFileSystemPath { get; set; }
        public string executableFileSystemPath { get; set; }
        public string narCsPath { get; set; }
        public string overlayModulePath { get; set; }
        public string baseRomCode { get; set; }
    }
}