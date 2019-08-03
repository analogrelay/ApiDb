using System;
using System.Collections.Generic;
using System.Diagnostics;
using ApiDb.Model;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApiDb
{
    public class IndexWalker : MetadataWalker
    {
        private readonly ILogger<IndexWalker> _logger;
        private AssemblyIdentity? _currentAssembly;
        private MetadataPath? _context;

        private List<ApiReference>? _refs;
        private List<ApiDeclaration>? _decls;

        public IndexWalker(ILogger<IndexWalker> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Walks the specified <see cref="AssemblyDefinition"/> and records new data in the index.
        /// </summary>
        /// <param name="assembly">The assembly to walk.</param>
        public AssemblyIndex IndexAssembly(AssemblyDefinition assembly, AssemblyDetails details)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (details is null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            Debug.Assert(_refs == null, "References list should be null");
            Debug.Assert(_decls == null, "Declarations list should be null");

            _refs = new List<ApiReference>();
            _decls = new List<ApiDeclaration>();
            _currentAssembly = AssemblyIdentity.ForAssembly(assembly);

            try
            {
                WalkAssembly(assembly);
            }
            finally
            {
                _currentAssembly = null;
            }

            var index = new AssemblyIndex(ModelVersion.Current, details, _refs, _decls);

            _currentAssembly = null;
            _refs = null;
            _decls = null;

            return index;
        }

        protected override void WalkAssembly(AssemblyDefinition assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var path = MetadataPath.ForAssembly(assembly);
            AddDeclaration(ApiDeclarationKind.Assembly, path);

            using (PushContext(path))
            {
                base.WalkAssembly(assembly);
            }
        }

        protected override void WalkType(TypeDefinition type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var path = MetadataPath.ForType(type);
            AddDeclaration(GetDeclarationKind(type), path);
            using (PushContext(path))
            {
                base.WalkType(type);
            }
        }

        protected override void VisitType(TypeDefinition type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Add references for base type and interfaces
            if (type.BaseType != null)
            {
                AddReference(MetadataPath.ForType(type.BaseType), ApiReferenceKind.Derive);
            }

            foreach (var iface in type.Interfaces)
            {
                AddReference(MetadataPath.ForType(iface.InterfaceType), ApiReferenceKind.Derive);
            }
        }

        protected override void WalkEvent(EventDefinition evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            var path = MetadataPath.ForEvent(evt);
            AddDeclaration(ApiDeclarationKind.Event, path);
            using (PushContext(path))
            {
                base.WalkEvent(evt);
            }
        }

        protected override void WalkField(FieldDefinition field)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var path = MetadataPath.ForField(field);
            AddDeclaration(ApiDeclarationKind.Field, path);
            using (PushContext(path))
            {
                base.WalkField(field);
            }
        }

        protected override void WalkMethod(MethodDefinition method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var path = MetadataPath.ForMethod(method);
            AddDeclaration(ApiDeclarationKind.Method, path);
            using (PushContext(path))
            {
                base.WalkMethod(method);
            }
        }

        protected override void WalkProperty(PropertyDefinition prop)
        {
            if (prop is null)
            {
                throw new ArgumentNullException(nameof(prop));
            }

            var path = MetadataPath.ForProperty(prop);
            AddDeclaration(ApiDeclarationKind.Property, path);
            using (PushContext(path))
            {
                base.WalkProperty(prop);
            }
        }

        protected override void WalkParameter(ParameterDefinition param)
        {
            if (param is null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            AddReference(MetadataPath.ForType(param.ParameterType), ApiReferenceKind.Reference);
        }

        protected override void VisitEvent(EventDefinition evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            AddReference(MetadataPath.ForType(evt.EventType), ApiReferenceKind.Reference);
        }

        protected override void VisitField(FieldDefinition field)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            AddReference(MetadataPath.ForType(field.FieldType), ApiReferenceKind.Reference);
        }

        protected override void VisitProperty(PropertyDefinition prop)
        {
            if (prop is null)
            {
                throw new ArgumentNullException(nameof(prop));
            }

            AddReference(MetadataPath.ForType(prop.PropertyType), ApiReferenceKind.Reference);
        }

        protected override void VisitLocal(VariableDefinition local)
        {
            if (local is null)
            {
                throw new ArgumentNullException(nameof(local));
            }

            AddReference(MetadataPath.ForType(local.VariableType), ApiReferenceKind.Reference);
        }

        protected override void VisitInstruction(Instruction instruction)
        {
            if (instruction is null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }

            _logger.LogTrace("Visiting: {Instruction}", instruction);
            switch (instruction.Operand)
            {
                case TypeReference typeRef:
                    AddReference(MetadataPath.ForType(typeRef), ApiReferenceKind.Reference);
                    break;
                case CallSite callSite:
                    _logger.LogWarning($"Encountered a CallSite: {callSite.FullName}");
                    break;
                case MethodReference methodRef:
                    AddReference(MetadataPath.ForMethod(methodRef), ApiReferenceKind.Reference);
                    break;
                case FieldReference fieldRef:
                    AddReference(MetadataPath.ForField(fieldRef), ApiReferenceKind.Reference);
                    break;
            }
        }

        protected override void VisitCustomAttribute(CustomAttribute attribute, ICustomAttributeProvider parent)
        {
            if (attribute is null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var attributeTypeId = MetadataPath.ForType(attribute.AttributeType);
            AddReference(attributeTypeId, ApiReferenceKind.Reference);

            AddReference(
                MetadataPath.ForMethod(attribute.Constructor),
                ApiReferenceKind.Reference);
        }

        private void AddReference(MetadataPath target, ApiReferenceKind kind)
        {
            var reference = new ApiReference(
                _currentAssembly ?? throw new InvalidOperationException("No current assembly!"),
                _context ?? throw new InvalidOperationException("No current context!"),
                target,
                kind);
            _logger.LogTrace("Adding reference: {ApiReference}", reference);
            _refs!.Add(reference);
        }

        private void AddDeclaration(ApiDeclarationKind kind, MetadataPath path)
        {
            var decl = new ApiDeclaration(kind, path);
            _logger.LogTrace("Adding declaration: {ApiDeclaration}", decl);
            _decls!.Add(decl);
        }

        private ApiDeclarationKind GetDeclarationKind(TypeDefinition type)
        {
            if (type.IsClass)
            {
                return ApiDeclarationKind.Class;
            }
            else if (type.IsEnum)
            {
                return ApiDeclarationKind.Enum;
            }
            else if (type.IsInterface)
            {
                return ApiDeclarationKind.Interface;
            }
            else if (type.IsValueType)
            {
                return ApiDeclarationKind.Struct;
            }
            else
            {
                return ApiDeclarationKind.Unknown;
            }
        }

        private Disposable<(MetadataPath?, MetadataPath, IDisposable)> PushContext(MetadataPath newIdentity)
        {
            var oldContext = _context;
            var scope = _logger.BeginScope(newIdentity);
            _context = newIdentity;
            return Disposable.Create((s) =>
            {
                var (prevContext, currentContext, loggerScope) = s;
                Debug.Assert(ReferenceEquals(currentContext, _context), "Context stack mismatch");
                _context = prevContext;
                loggerScope.Dispose();
            }, (oldContext, _context, scope));
        }
    }
}
