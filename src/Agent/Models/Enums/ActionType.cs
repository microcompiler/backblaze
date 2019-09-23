
namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Specifies the file action type.
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Indicates a large file has been started but not finished or canceled.
        /// </summary>
        Start,

        /// <summary>
        /// Indicates a file that was uploaded to cloud storage.
        /// </summary>
        Upload,

        /// <summary>
        /// Indicates the file as hidden it will not show up in list file names.
        /// </summary>
        Hide,

        /// <summary>
        /// Indicates a virtual folder when listing files.
        /// </summary>
        Folder,

        /// <summary>
        /// Indicates a file that was copied from an existing file.
        /// </summary>
        Copy
    }
}