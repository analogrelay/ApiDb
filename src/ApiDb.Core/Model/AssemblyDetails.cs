using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents details about the assembly.
    /// </summary>
    public class AssemblyDetails
    {
        /// <summary>
        /// Constructs a new <see cref="AssemblyDetails"/>
        /// </summary>
        /// <param name="identity">The unique identity of the assembly.</param>
        /// <param name="informationalVersion">The value of the <see cref="AssemblyInformationalVersionAttribute"/> for this assembly, if known.</param>
        /// <param name="build">The build identity of the assembly, if known.</param>
        public AssemblyDetails(AssemblyIdentity identity, string? informationalVersion = null, BuildIdentity? build = null)
        {
            Identity = identity;
            InformationalVersion = informationalVersion;
            Build = build;
        }

        /// <summary>
        /// Gets the unique identity of the assembly.
        /// </summary>
        public AssemblyIdentity Identity { get; }

        /// <summary>
        /// Gets the value of the <see cref="AssemblyInformationalVersionAttribute"/> for this assembly, if known.
        /// </summary>
        public string? InformationalVersion { get; }

        /// <summary>
        /// Gets the build identity of the assembly, if known.
        /// </summary>
        public BuildIdentity? Build { get; }

        public static AssemblyDetails ForAssembly(AssemblyDefinition asm)
        {
            return new AssemblyDetails(
                AssemblyIdentity.ForAssembly(asm),
                GetInformationalVersion(asm));
        }

        private static string? GetInformationalVersion(AssemblyDefinition asm)
            => asm.CustomAttributes.GetByLocalType(typeof(AssemblyInformationalVersionAttribute))
                .FirstOrDefault()
                ?.GetConstructorValues<string>();
    }
}
