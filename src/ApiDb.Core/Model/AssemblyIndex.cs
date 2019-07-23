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
        /// <param name="apiReferences">A list of <see cref="ApiReference"/> objects describing the APIs this assembly references.</param>
        /// <param name="types">A list of <see cref="ApiDeclaration"/> objects describing the types declared in this assembly (and their members).</param>
        public AssemblyIndex(AssemblyDetails details, IReadOnlyList<ApiReference> apiReferences, IReadOnlyList<ApiDeclaration> apiDeclarations)
        {
            Details = details ?? throw new System.ArgumentNullException(nameof(details));
            ApiReferences = apiReferences ?? throw new System.ArgumentNullException(nameof(apiReferences));
            ApiDeclarations = apiDeclarations ?? throw new System.ArgumentNullException(nameof(apiDeclarations));
        }

        /// <summary>
        /// Gets the <see cref="AssemblyDetails"/> describing the detailed identity of the assembly.
        /// </summary>
        public AssemblyDetails Details { get; }

        /// <summary>
        /// Gets a list of <see cref="ApiReference"/> objects describing the APIs this assembly references.
        /// </summary>
        public IReadOnlyList<ApiReference> ApiReferences { get; }

        /// <summary>
        /// Gets a list of <see cref="ApiDeclaration"/> objects describing the types declared in this assembly (and their members).
        /// </summary>
        public IReadOnlyList<ApiDeclaration> ApiDeclarations { get; }
    }
}
