using System;
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
        /// <param name="version">A <see cref="ModelVersion"/> specifying the version of the API catalog model.</param>
        /// <param name="details">The <see cref="AssemblyDetails"/> describing the detailed identity of the assembly.</param>
        /// <param name="apiDeclarations">A list of <see cref="ApiDeclaration"/> objects describing the APIs this assembly declares.</param>
        /// <param name="apiReferences">A list of <see cref="ApiReference"/> objects describing the APIs this assembly references.</param>
        /// <param name="types">A list of <see cref="ApiDeclaration"/> objects describing the types declared in this assembly (and their members).</param>
        public AssemblyIndex(ModelVersion version, AssemblyDetails details, IReadOnlyList<ApiReference> apiReferences, IReadOnlyList<ApiDeclaration> apiDeclarations)
        {
            Version = version != ModelVersion.Empty
                ? version
                : throw new ArgumentException("Model version cannot be empty", nameof(version));
            Details = details ?? throw new ArgumentNullException(nameof(details));
            ApiReferences = apiReferences ?? throw new ArgumentNullException(nameof(apiReferences));
            ApiDeclarations = apiDeclarations ?? throw new ArgumentNullException(nameof(apiDeclarations));
        }

        /// <summary>
        /// Gets a <see cref="ModelVersion"/> specifying the version of the API catalog model.
        /// </summary>
        public ModelVersion Version { get; }

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
