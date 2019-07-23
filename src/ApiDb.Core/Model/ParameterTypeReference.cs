using Mono.Cecil;

namespace ApiDb.Model
{
    /// <summary>
    /// Describes the type referenced by a parameter.
    /// </summary>
    public class ParameterTypeReference
    {
        /// <summary>
        /// Constructs a <see cref="ParameterTypeReference"/> referring to a specific type.
        /// </summary>
        /// <param name="metadataReference">A <see cref="MetadataPath"/> describing the type referenced.</param>
        public ParameterTypeReference(MetadataPath metadataReference)
        {
            Type = metadataReference;
        }

        /// <summary>
        /// Constructs a <see cref="ParameterTypeReference"/> referring to a generic parameter.
        /// </summary>
        /// <param name="position">The position of the generic parameter being referenced.</param>
        public ParameterTypeReference(int position)
        {
            GenericParameterPosition = position;
        }

        /// <summary>
        /// Gets the position of the generic parameter being referenced.
        /// </summary>
        public int? GenericParameterPosition { get; }

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> describing the type referenced.
        /// </summary>
        public MetadataPath? Type { get; }

        public override string ToString() => GenericParameterPosition is int pos ? $"!{pos}" : Type!.ToString();
    }
}