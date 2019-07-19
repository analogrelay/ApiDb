using System.Collections.Generic;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents index data for a specific assembly
    /// </summary>
    public class AssemblyIndex
    {
        /// <summary>
        /// Constructs an <see cref="AssemblyIndex"/>.
        /// </summary>
        /// <param name="details">The <see cref="AssemblyDetails"/> describing the detailed identity of the assembly.</param>
        /// <param name="apiReferences">A list of <see cref="ApiReference"/> describing the APIs this assembly references.</param>
        public AssemblyIndex(AssemblyDetails details, IReadOnlyList<ApiReference> apiReferences)
        {
            Details = details;
            ApiReferences = apiReferences;
        }

        /// <summary>
        /// Gets the <see cref="AssemblyDetails"/> describing the detailed identity of the assembly.
        /// </summary>
        public AssemblyDetails Details { get; }

        /// <summary>
        /// Gets a list of <see cref="ApiReference"/> describing the APIs this assembly references.
        /// </summary>
        public IReadOnlyList<ApiReference> ApiReferences { get; }
    }
}
