namespace ApiDb.Model
{
    /// <summary>
    /// Describes an API declaration.
    /// </summary>
    public class ApiDeclaration
    {
        /// <summary>
        /// Constructs an <see cref="ApiDeclaration"/>
        /// </summary>
        /// <param name="kind">A <see cref="ApiDeclarationKind"/> identifying the kind of API declaration.</param>
        /// <param name="path">A <see cref="MetadataPath"/> identifying the API that is defined.</param>
        public ApiDeclaration(ApiDeclarationKind kind, MetadataPath path)
        {
            Kind = kind;
            Path = path;
        }

        /// <summary>
        /// Gets a <see cref="ApiDeclarationKind"/> identifying the kind of API declaration.
        /// </summary>
        public ApiDeclarationKind Kind { get; }

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> identifying the API that is defined.
        /// </summary>
        public MetadataPath Path { get; }
    }
}