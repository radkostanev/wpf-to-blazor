namespace OutlookInspiredApp.Blazor.Models
{
    /// <summary>
    /// Email folder (hierarchical structure)
    /// </summary>
    public class Folder
    {
        public string FolderID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int UnreadCount { get; set; } = 0;
        public string ParentFolderID { get; set; } = string.Empty;
        
        public List<Folder> SubFolders { get; set; } = new List<Folder>();

        public bool HasChildren => SubFolders?.Count > 0;
    }
}
