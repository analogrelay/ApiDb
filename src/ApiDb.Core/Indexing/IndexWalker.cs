using System;
using System.Collections.Generic;
using System.Diagnostics;
using ApiDb.Model;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApiDb.Indexing
{
    public class IndexWalker : MetadataWalker
    {
        private readonly ILogger<IndexWalker> _logger;
        private AssemblyIdentity? _currentAssembly;
        private MetadataReference? _context;

        private List<ApiReference> _apis = new List<ApiReference>();

        public IReadOnlyList<ApiReference> IndexedApis => _apis;

        public IndexWalker(ILogger<IndexWalker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Walks the specified <see cref="AssemblyDefinition"/> and records new data in the index.
        /// </summary>
        /// <param name="assembly">The assembly to walk.</param>
        public AssemblyIndex IndexAssembly(AssemblyDefinition assembly, AssemblyDetails details)
        {
            _currentAssembly = AssemblyIdentity.ForAssembly(assembly);
            try
            {
                WalkAssembly(assembly);
            }
            finally
            {
                _currentAssembly = null;
            }

            var apis = _apis;
            _apis = new List<ApiReference>();
            _currentAssembly = null;

            return new AssemblyIndex(details, apis);
        }

        protected override void WalkAssembly(AssemblyDefinition assembly)
        {
            using (PushContext(MetadataReference.ForAssembly(assembly)))
            {
                base.WalkAssembly(assembly);
            }
        }

        protected override void WalkType(TypeDefinition type)
        {
            using (PushContext(MetadataReference.ForType(type)))
            {
                base.WalkType(type);
            }
        }

        protected override void VisitType(TypeDefinition type)
        {
            // Add references for base type and interfaces
            if (type.BaseType != null)
            {
                AddReference(MetadataReference.ForType(type.BaseType), ApiReferenceKind.Derive);
            }

            foreach (var iface in type.Interfaces)
            {
                AddReference(MetadataReference.ForType(iface.InterfaceType), ApiReferenceKind.Derive);
            }
        }

        protected override void WalkEvent(EventDefinition evt)
        {
            using (PushContext(MetadataReference.ForEvent(evt)))
            {
                base.WalkEvent(evt);
            }
        }

        protected override void WalkField(FieldDefinition field)
        {
            using (PushContext(MetadataReference.ForField(field)))
            {
                base.WalkField(field);
            }
        }

        protected override void WalkMethod(MethodDefinition method)
        {
            using (PushContext(MetadataReference.ForMethod(method)))
            {
                base.WalkMethod(method);
            }
        }

        protected override void WalkProperty(PropertyDefinition prop)
        {
            using (PushContext(MetadataReference.ForProperty(prop)))
            {
                base.WalkProperty(prop);
            }
        }

        protected override void WalkParameter(ParameterDefinition param)
        {
            using (PushContext(MetadataReference.ForParameter(_context ?? throw new InvalidOperationException("Requires an active context"), param)))
            {
                base.WalkParameter(param);
            }
        }

        protected override void VisitEvent(EventDefinition evt)
        {
            AddReference(MetadataReference.ForType(evt.EventType), ApiReferenceKind.Reference);
        }

        protected override void VisitField(FieldDefinition field)
        {
            AddReference(MetadataReference.ForType(field.FieldType), ApiReferenceKind.Reference);
        }

        protected override void VisitProperty(PropertyDefinition prop)
        {
            AddReference(MetadataReference.ForType(prop.PropertyType), ApiReferenceKind.Reference);
        }

        protected override void VisitLocal(VariableDefinition local)
        {
            AddReference(MetadataReference.ForType(local.VariableType), ApiReferenceKind.Reference);
        }

        protected override void VisitInstruction(Instruction instruction)
        {
            _logger.LogTrace("Visiting: {Instruction}", instruction);
            switch(instruction.Operand)
            {
                case TypeReference typeRef:
                    AddReference(MetadataReference.ForType(typeRef), ApiReferenceKind.Reference);
                    break;
                case CallSite callSite:
                    _logger.LogWarning($"Encountered a CallSite: {callSite.FullName}");
                    break;
                case MethodReference methodRef:
                    AddReference(MetadataReference.ForMethod(methodRef), ApiReferenceKind.Reference);
                    break;
                case FieldReference fieldRef:
                    AddReference(MetadataReference.ForField(fieldRef), ApiReferenceKind.Reference);
                    break;
            }
        }

        protected override void VisitCustomAttribute(CustomAttribute attribute, ICustomAttributeProvider parent)
        {
            var attributeTypeId = MetadataReference.ForType(attribute.AttributeType);
            AddReference(attributeTypeId, ApiReferenceKind.Reference);

            AddReference(
                MetadataReference.ForMethod(attribute.Constructor),
                ApiReferenceKind.Reference);
        }

        private void AddReference(MetadataReference target, ApiReferenceKind kind)
        {
            var reference = new ApiReference(
                _currentAssembly ?? throw new InvalidOperationException("No current assembly!"),
                _context ?? throw new InvalidOperationException("No current context!"),
                target,
                kind);
            _logger.LogTrace("Adding reference: {ApiReference}", reference);
            _apis.Add(reference);
        }

        private Disposable<(MetadataReference?, MetadataReference, IDisposable)> PushContext(MetadataReference newIdentity)
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
