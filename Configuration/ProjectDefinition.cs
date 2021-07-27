namespace Hotswap.Configuration
{
    public class ProjectDefinition
    {
        // Attributes that are customizable by the user.
        public string ProjectName { get; set; }
        public string ProjectROMCode { get; set; }
        public string ProjectGameTitle { get; set; }

        // General attributes for Hotswap.
        public string ROMFileSystemPath { get; set; }
        public string ExecutableFileSystemPath { get; set; }
        public string NARCsPath { get; set; }
        public string OverlayModulePath { get; set; }
        public string BaseROMCode { get; set; }
    }
}