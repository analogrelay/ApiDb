using System;
using System.Reflection;
using Mono.Cecil;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents the identity of a specific assembly at a specific version.
    /// </summary>
    public class AssemblyIdentity : IEquatable<AssemblyIdentity>
    {
        /// <summary>
        /// Constructs a new <see cref="AssemblyIdentity"/>
        /// </summary>
        /// <param name="uniqueId">A unique identifier for the assembly, guaranteed to change if the content changes.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> for the assembly.</param>
        public AssemblyIdentity(string uniqueId, AssemblyName assemblyName)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                throw new ArgumentException("message", nameof(uniqueId));
            }

            UniqueId = uniqueId;
            AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }

        /// <summary>
        /// Gets a unique identifier for the assembly, guaranteed to change if the functional content of the assembly changes.
        /// </summary>
        /// <remarks>
        /// A hash is a good identity to use, but be aware of crossgen and other tools that change the physical on-disk content of an assembly
        /// without changing the functional content. A Module Version ID (<see cref="ModuleDefinition.Mvid"/>) is also a good identity to use, though it will change on every build even if the
        /// input content is the same (unless using deterministic compilation).
        /// </remarks>
        public string UniqueId { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> for the assembly.
        /// </summary>
        public AssemblyName AssemblyName { get; }

        /// <summary>
        /// Creates an <see cref="AssemblyIdentity"/> for the specified assembly.
        /// </summary>
        /// <param name="asm">The <see cref="AssemblyDefinition"/> to create an <see cref="AssemblyIdentity"/> for.</param>
        /// <returns>The identity.</returns>
        /// <remarks>
        /// This overload uses the <see cref="ModuleDefinition.Mvid"/> of the <see cref="AssemblyDefinition.MainModule"/> as
        /// the <see cref="UniqueId"/>.
        /// </remarks>
        public static AssemblyIdentity ForAssembly(AssemblyDefinition asm)
        {
            return new AssemblyIdentity(
                asm.MainModule.Mvid.ToString("N"),
                new AssemblyName(asm.FullName));
        }

        public bool Equals(AssemblyIdentity other) => string.Equals(other.UniqueId, UniqueId);

        public override bool Equals(object obj) => obj is AssemblyIdentity other && Equals(other);

        public override int GetHashCode() => UniqueId.GetHashCode();

        public override string ToString() => $"{AssemblyName.Name}@{UniqueId}";
    }
}
