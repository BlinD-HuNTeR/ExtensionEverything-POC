using Blur;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = true)]
class ConstraintAttribute : Attribute, IGenericParameterWeaver
{
    public bool RefOnly;
    TypeReference constraintType;

    public ConstraintAttribute(Type type) { }
    public ConstraintAttribute(TypeReference type) => constraintType = type;

    public void Apply(GenericParameter parameter, MethodDefinition method)
    {
        if (Extensions.RefAssembly != null)
        {
            var module = Extensions.ReadAssembly(Extensions.RefAssembly).MainModule;
            MetadataResolver.GetMethod(module.GetType(method.DeclaringType.FullName).Methods, method).GenericParameters[parameter.Position].Constraints.Add(module.ImportReference(constraintType));
        }
        if (!RefOnly) parameter.Constraints.Add(method.Module.ImportReference(constraintType));
    }

    public void Apply(GenericParameter parameter, TypeDefinition type)
    {
        if (Extensions.RefAssembly != null)
        {
            var module = Extensions.ReadAssembly(Extensions.RefAssembly).MainModule;
            module.GetType(type.FullName).GenericParameters[parameter.Position].Constraints.Add(module.ImportReference(constraintType));
        }
        if (!RefOnly) parameter.Constraints.Add(type.Module.ImportReference(constraintType));
    }
}

[AttributeUsage(AttributeTargets.Class)]
class InvocableAttribute : Attribute, ITypeWeaver
{
    public void Apply(TypeDefinition type)
    {
        var module = Extensions.ReadAssembly(Extensions.RefAssembly).MainModule;
        module.GetType(type.FullName).BaseType = module.ImportReference(typeof(MulticastDelegate).GetReference());
    }
}
