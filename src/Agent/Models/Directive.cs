namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Specifies the strategy to copy files.
    /// </summary>
    public enum Directive
    {
        /// <summary>
        /// Indicates when copying files the new file's metadata to be equal to the source file's metadata.
        /// </summary>
        Copy,

        /// <summary>
        /// Indicates when copying files the source file's metadata will be ignored and set to what you provide.
        /// </summary>
        Replace
    }
}