namespace ApiDb.Model
{
    /// <summary>
    /// Represents a build identity, which is the identity of a specific build.
    /// </summary>
    public class BuildIdentity
    {
        /// <summary>
        /// Gets the source repository used in the build.
        /// </summary>
        public string? Repository { get; set; }

        /// <summary>
        /// Gets the source version used in the build (i.e. commit hash).
        /// </summary>
        public string? SourceVersion { get; set; }

        /// <summary>
        /// Gets a url to the sources used in the build.
        /// </summary>
        public string? SourceUrl { get; set; }

        /// <summary>
        /// Gets the name of the branch used in the build.
        /// </summary>
        public string? Branch { get; set; }

        /// <summary>
        /// Gets the name of the build.
        /// </summary>
        public string? BuildName { get; set; }

        /// <summary>
        /// Gets the number of the build.
        /// </summary>
        public string? BuildNumber { get; set; }
    }
}