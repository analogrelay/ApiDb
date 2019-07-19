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
        /// <param name="source">A <see cref="MetadataReference"/> referring to the metadata item containing the reference.</param>
        /// <param name="target">A <see cref="MetadataReference"/> representing the target item.</param>
        /// <param name="kind">A <see cref="ApiReferenceKind"/> representing the kind of the API reference.</param>
        public ApiReference(AssemblyIdentity assembly, MetadataReference source, MetadataReference target, ApiReferenceKind kind)
        {
            Assembly = assembly;
            Source = source;
            Target = target;
            Kind = kind;
        }

        /// <summary>
        /// Gets the assembly containing the reference.
        /// </summary>
        public AssemblyIdentity Assembly { get; }

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> referring to the metadata item containing the reference.
        /// </summary>
        public MetadataReference Source { get; }

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> representing the target item.
        /// </summary>
        public MetadataReference Target { get; }

        /// <summary>
        /// Gets the <see cref="ApiReferenceKind"/> defining the kind of the API reference.
        /// </summary>
        public ApiReferenceKind Kind { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"[{Kind}]");
            builder.Append(Assembly.ToString());
            if(!string.IsNullOrEmpty(Source.TypeName))
            {
                builder.Append($"!{Source.TypeName}");
                if(!string.IsNullOrEmpty(Source.MemberName))
                {
                    builder.Append($"#{Source.MemberName}");
                    if(!string.IsNullOrEmpty(Source.ParameterName))
                    {
                        builder.Append($"/{Source.ParameterName}");
                    }
                }
            }

            builder.Append(" ==> ");
            builder.Append(Target.ToString());
            return builder.ToString();
        }
    }
}
