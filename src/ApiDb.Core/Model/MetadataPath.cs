using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace ApiDb.Model
{
    /// <summary>
    /// Represents an reference to a metadata item.
    /// </summary>
    public class MetadataPath
    {
        /// <summary>
        /// Constructs a new <see cref="MetadataPath"/> referring to a parameter.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="typeName">The fully-qualified type name of the assembly.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameterTypes">A list of argument types (if the metadata item is a member with arguments.</param>
        public MetadataPath(AssemblyName assemblyName, string? moduleName, string? typeName, string? memberName, IReadOnlyList<ParameterTypeReference>? parameterTypes)
        {
            AssemblyName = assemblyName;
            ModuleName = moduleName ?? string.Empty;
            TypeName = typeName ?? string.Empty;
            MemberName = memberName ?? string.Empty;
            ParameterTypes = parameterTypes ?? Array.Empty<ParameterTypeReference>();
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public AssemblyName AssemblyName { get; }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string? ModuleName { get; }

        /// <summary>
        /// Gets the fully-qualified type name of the assembly (if the metadata item is a type or member).
        /// </summary>
        public string? TypeName { get; }

        /// <summary>
        /// Gets the name of the member (if the metadata item is a member).
        /// </summary>
        public string? MemberName { get; }

        /// <summary>
        /// Gets a list of argument types (if the metadata item is a member with arguments.
        /// </summary>
        public IReadOnlyList<ParameterTypeReference> ParameterTypes { get; }

        /// <summary>
        /// Gets the name of the parameter (if the metadata item is a parameter).
        /// </summary>
        public string? ParameterName { get; }

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="AssemblyDefinition"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the assembly.</returns>
        public static MetadataPath ForAssembly(AssemblyDefinition assembly) => new MetadataPath(
            GetAssemblyName(assembly.Name),
            moduleName: null,
            typeName: null,
            memberName: null,
            parameterTypes: null);

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="TypeReference"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the type.</returns>
        public static MetadataPath ForType(TypeReference type) => new MetadataPath(
            GetAssemblyName(type.Scope),
            GetModuleName(type.Scope),
            type.FullName,
            memberName: null,
            parameterTypes: null);

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified method.
        /// </summary>
        /// <param name="type">The <see cref="MethodReference"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the method.</returns>
        public static MetadataPath ForMethod(MethodReference method) => new MetadataPath(
            GetAssemblyName(method.DeclaringType.Scope),
            GetModuleName(method.DeclaringType.Scope),
            method.DeclaringType.FullName,
            method.Name,
            GetParameterReferences(method));

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified event
        /// </summary>
        /// <param name="evt">The <see cref="EventDefinition"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the event.</returns>
        public static MetadataPath ForEvent(EventDefinition evt) => new MetadataPath(
            GetAssemblyName(evt.DeclaringType.Scope),
            GetModuleName(evt.DeclaringType.Scope),
            evt.DeclaringType.FullName,
            evt.Name,
            parameterTypes: null);

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified field
        /// </summary>
        /// <param name="evt">The <see cref="FieldReference"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the field.</returns>
        public static MetadataPath ForField(FieldReference field) => new MetadataPath(
            GetAssemblyName(field.DeclaringType.Scope),
            GetModuleName(field.DeclaringType.Scope),
            field.DeclaringType.FullName,
            field.Name,
            parameterTypes: null);

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified field
        /// </summary>
        /// <param name="evt">The <see cref="FieldDefinition"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the field.</returns>
        public static MetadataPath ForField(FieldDefinition field) => new MetadataPath(
            GetAssemblyName(field.DeclaringType.Scope),
            GetModuleName(field.DeclaringType.Scope),
            field.DeclaringType.FullName,
            field.Name,
            parameterTypes: null);

        /// <summary>
        /// Gets a <see cref="MetadataPath"/> that refers to the specified property
        /// </summary>
        /// <param name="evt">The <see cref="PropertyDefinition"/> to generate a <see cref="MetadataPath"/> for.</param>
        /// <returns>A <see cref="MetadataPath"/> referring to the property.</returns>
        public static MetadataPath ForProperty(PropertyDefinition prop) => new MetadataPath(
            GetAssemblyName(prop.DeclaringType.Scope),
            GetModuleName(prop.DeclaringType.Scope),
            prop.DeclaringType.FullName,
            prop.Name,
            parameterTypes: null);

        private static IReadOnlyList<ParameterTypeReference> GetParameterReferences(MethodReference method)
        {
            return method.Parameters.Select(p =>
                p.ParameterType is GenericParameter genericParam
                    ? new ParameterTypeReference(genericParam.Position)
                    : new ParameterTypeReference(ForType(p.ParameterType))).ToList();
        }

        private static string? GetModuleName(IMetadataScope scope) => scope switch
        {
            AssemblyNameReference _ => null,
            ModuleDefinition modDef => modDef.Name,
            _ => throw new InvalidOperationException($"Unsupported metadata scope type '{scope.MetadataScopeType}' (Type Name: {scope.GetType()})."),
        };

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
            if (!string.IsNullOrEmpty(ModuleName))
            {
                builder.Append($":{ModuleName}");
            }

            if (!string.IsNullOrEmpty(TypeName))
            {
                builder.Append($"/{TypeName}");
                if (!string.IsNullOrEmpty(MemberName))
                {
                    builder.Append($"#{MemberName}");
                    if (ParameterTypes.Count > 0)
                    {
                        builder.Append("(");
                        foreach (var paramType in ParameterTypes)
                        {
                            builder.Append($"{paramType}, ");
                        }
                        builder.Length -= 2; // Remove trailing ", "
                        builder.Append(")");
                    }
                }
            }

            return builder.ToString();
        }
    }
}
