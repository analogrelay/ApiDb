using System.Text;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents a reference to an API.
    /// </summary>
    public class ApiReference
    {
        /// <summary>
        /// Constructs a new <see cref="ApiReference"/>
        /// </summary>
        /// <param name="assembly">The assembly containing the reference.</param>
        /// <param name="source">A <see cref="MetadataPath"/> referring to the metadata item containing the reference.</param>
        /// <param name="target">A <see cref="MetadataPath"/> representing the target item.</param>
        /// <param name="kind">A <see cref="ApiReferenceKind"/> representing the kind of the API reference.</param>
        public ApiReference(AssemblyIdentity assembly, MetadataPath source, MetadataPath target, ApiReferenceKind kind)
        {
            Assembly = assembly ?? throw new System.ArgumentNullException(nameof(assembly));
            Source = source ?? throw new System.ArgumentNullException(nameof(source));
            Target = target ?? throw new System.ArgumentNullException(nameof(target));
            Kind = kind;
        }

        /// <summary>
        /// Gets the assembly containing the reference.
        /// </summary>
        public AssemblyIdentity Assembly { get; }

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> referring to the metadata item containing the reference.
        /// </summary>
        public MetadataPath Source { get; }

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> representing the target item.
        /// </summary>
        public MetadataPath Target { get; }

        /// <summary>
        /// Gets the <see cref="ApiReferenceKind"/> defining the kind of the API reference.
        /// </summary>
        public ApiReferenceKind Kind { get; }
    }
}
