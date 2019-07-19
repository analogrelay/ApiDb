using System;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents an reference to a metadata item.
    /// </summary>
    public class MetadataReference
    {
        /// <summary>
        /// Constructs a new <see cref="MetadataReference"/> referring to an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        public MetadataReference(AssemblyName assemblyName)
        {
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Constructs a new <see cref="MetadataReference"/> referring to a type.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="typeName">The fully-qualified type name of the assembly.</param>
        public MetadataReference(AssemblyName assemblyName, string typeName)
            : this(assemblyName)
        {
            TypeName = typeName;
        }

        /// <summary>
        /// Constructs a new <see cref="MetadataReference"/> referring to a member.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="typeName">The fully-qualified type name of the assembly.</param>
        /// <param name="memberName">The name of the member.</param>
        public MetadataReference(AssemblyName assemblyName, string typeName, string memberName)
            : this(assemblyName, typeName)
        {
            MemberName = memberName;
        }

        /// <summary>
        /// Constructs a new <see cref="MetadataReference"/> referring to a parameter.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="typeName">The fully-qualified type name of the assembly.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        public MetadataReference(AssemblyName assemblyName, string typeName, string memberName, string parameterName)
            : this(assemblyName, typeName, memberName)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public AssemblyName AssemblyName { get; }

        /// <summary>
        /// Gets the fully-qualified type name of the assembly (if the metadata item is a type or member).
        /// </summary>
        public string? TypeName { get; }

        /// <summary>
        /// Gets the name of the member (if the metadata item is a member).
        /// </summary>
        public string? MemberName { get; }

        /// <summary>
        /// Gets the name of the parameter (if the metadata item is a parameter).
        /// </summary>
        public string? ParameterName { get; }

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="AssemblyDefinition"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the assembly.</returns>
        public static MetadataReference ForAssembly(AssemblyDefinition assembly) => new MetadataReference(GetAssemblyName(assembly.Name));

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="TypeReference"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the type.</returns>
        public static MetadataReference ForType(TypeReference type) => new MetadataReference(
            GetAssemblyName(type.Scope),
            type.FullName);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified method.
        /// </summary>
        /// <param name="type">The <see cref="MethodReference"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the method.</returns>
        public static MetadataReference ForMethod(MethodReference method) => new MetadataReference(
            GetAssemblyName(method.DeclaringType.Scope),
            method.DeclaringType.FullName,
            method.Name);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified event
        /// </summary>
        /// <param name="evt">The <see cref="EventDefinition"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the event.</returns>
        public static MetadataReference ForEvent(EventDefinition evt) => new MetadataReference(
                GetAssemblyName(evt.DeclaringType.Scope),
                evt.DeclaringType.FullName,
                evt.Name);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified field
        /// </summary>
        /// <param name="evt">The <see cref="FieldReference"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the field.</returns>
        public static MetadataReference ForField(FieldReference field) => new MetadataReference(
                GetAssemblyName(field.DeclaringType.Scope),
                field.DeclaringType.FullName,
                field.Name);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified field
        /// </summary>
        /// <param name="evt">The <see cref="FieldDefinition"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the field.</returns>
        public static MetadataReference ForField(FieldDefinition field) => new MetadataReference(
                GetAssemblyName(field.DeclaringType.Scope),
                field.DeclaringType.FullName,
                field.Name);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified property
        /// </summary>
        /// <param name="evt">The <see cref="PropertyDefinition"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the property.</returns>
        public static MetadataReference ForProperty(PropertyDefinition prop) => new MetadataReference(
                GetAssemblyName(prop.DeclaringType.Scope),
                prop.DeclaringType.FullName,
                prop.Name);

        /// <summary>
        /// Gets a <see cref="MetadataReference"/> that refers to the specified parameter
        /// </summary>
        /// <param name="method">A <see cref="MetadataReference"/> referring to the method containing this parameter.</param>
        /// <param name="param">The <see cref="ParameterDefinition"/> to generate a <see cref="MetadataReference"/> for.</param>
        /// <returns>A <see cref="MetadataReference"/> referring to the parameter.</returns>
        public static MetadataReference ForParameter(MetadataReference method, ParameterDefinition param) => new MetadataReference(
                method.AssemblyName,
                method.TypeName ?? throw new ArgumentException("Must be a metadata reference to a method", nameof(method)),
                method.MemberName ?? throw new ArgumentException("Must be a metadata reference to a method", nameof(method)),
                param.Name);

        private static AssemblyName GetAssemblyName(IMetadataScope scope) => scope switch
        {
            AssemblyNameReference asmName => new AssemblyName(asmName.FullName),
            ModuleDefinition modDef => new AssemblyName(modDef.Assembly.FullName),
            _ => throw new InvalidOperationException($"Unsupported metadata scope type '{scope.MetadataScopeType}' (Type Name: {scope.GetType()})."),
        };

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{AssemblyName.Name}[{AssemblyName.Version}]");
            if(!string.IsNullOrEmpty(TypeName))
            {
                builder.Append($"!{TypeName}");
                if(!string.IsNullOrEmpty(MemberName))
                {
                    builder.Append($"#{MemberName}");
                    if(!string.IsNullOrEmpty(ParameterName))
                    {
                        builder.Append($"/{ParameterName}");
                    }
                }
            }
            return builder.ToString();
        }
    }
}
