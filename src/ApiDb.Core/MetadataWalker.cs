using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApiDb
{
    public abstract class MetadataWalker
    {
        protected virtual void WalkAssembly(AssemblyDefinition assembly)
        {
            VisitAssembly(assembly);
            WalkCustomAttributes(assembly);

            foreach (var module in assembly.Modules)
            {
                WalkModule(module);
            }
        }

        protected virtual void WalkModule(ModuleDefinition module)
        {
            VisitModule(module);
            WalkCustomAttributes(module);

            foreach (var type in module.Types)
            {
                WalkType(type);
            }
        }

        protected virtual void WalkType(TypeDefinition type)
        {
            VisitType(type);
            WalkCustomAttributes(type);

            foreach (var field in type.Fields)
            {
                WalkField(field);
            }

            foreach (var prop in type.Properties)
            {
                WalkProperty(prop);
            }

            foreach (var method in type.Methods)
            {
                WalkMethod(method);
            }

            foreach (var evt in type.Events)
            {
                WalkEvent(evt);
            }
        }

        protected virtual void WalkEvent(EventDefinition evt)
        {
            WalkCustomAttributes(evt);
            VisitEvent(evt);

            if (evt.AddMethod != null)
            {
                WalkMethod(evt.AddMethod);
            }

            if (evt.RemoveMethod != null)
            {
                WalkMethod(evt.RemoveMethod);
            }
        }

        protected virtual void WalkMethod(MethodDefinition method)
        {
            WalkCustomAttributes(method);
            VisitMethod(method);

            foreach (var param in method.Parameters)
            {
                WalkParameter(param);
            }

            if (method.Body != null)
            {
                foreach (var local in method.Body.Variables)
                {
                    VisitLocal(local);
                }

                foreach (var instruction in method.Body.Instructions)
                {
                    VisitInstruction(instruction);
                }
            }
        }

        protected virtual void WalkParameter(ParameterDefinition param)
        {
            WalkCustomAttributes(param);
            VisitParameter(param);
        }

        protected virtual void WalkProperty(PropertyDefinition prop)
        {
            WalkCustomAttributes(prop);
            VisitProperty(prop);

            if (prop.GetMethod != null)
            {
                WalkMethod(prop.GetMethod);
            }

            if (prop.SetMethod != null)
            {
                WalkMethod(prop.SetMethod);
            }
        }

        protected virtual void WalkField(FieldDefinition field)
        {
            WalkCustomAttributes(field);
            VisitField(field);
        }

        protected virtual void VisitEvent(EventDefinition evt)
        {
        }

        protected virtual void VisitMethod(MethodDefinition method)
        {
        }

        protected virtual void VisitProperty(PropertyDefinition prop)
        {
        }

        protected virtual void VisitField(FieldDefinition field)
        {
        }

        protected virtual void VisitCustomAttribute(CustomAttribute attribute, ICustomAttributeProvider parent)
        {
        }

        protected virtual void VisitInstruction(Instruction instruction)
        {
        }

        protected virtual void VisitLocal(VariableDefinition local)
        {
        }

        protected virtual void VisitParameter(ParameterDefinition param)
        {
        }

        protected virtual void VisitType(TypeDefinition type)
        {
        }

        protected virtual void VisitModule(ModuleDefinition module)
        {
        }

        protected virtual void VisitAssembly(AssemblyDefinition assembly)
        {
        }

        private void WalkCustomAttributes(ICustomAttributeProvider attributeProvider)
        {
            foreach (var attribute in attributeProvider.CustomAttributes)
            {
                VisitCustomAttribute(attribute, attributeProvider);
            }
        }
    }
}
