using Blur;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: Blur(CleanUp = true, AdditionalUnreferencedAssemblies = new[]{ "Blur", "Mono.Cecil" })]

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
class ExtensionAttribute : Attribute, ITypeWeaver
{
    TypeReference type;
    public ExtensionAttribute(Type type) { }
    public ExtensionAttribute(TypeReference type) => this.type = type;

    public void Apply(TypeDefinition extensionType) => Extensions.SetBaseType(Extensions.GetDefinition(type), extensionType);
}

partial class Extensions : BlurVisitor, IWeaver
{
    static Dictionary<string, AssemblyDefinition> ReferencedAssemblies = new(), OtherAssemblies = new();
    static Dictionary<(string, Version), string> References = (from path in Blur.Processing.AssemblyResolver.References
                                                               select (name: AssemblyName.GetAssemblyName(path), path))
                                                               .ToDictionary(e => (e.name.FullName, e.name.Version), e => e.path);

    public static AssemblyDefinition GetReferencedAssembly(AssemblyNameReference reference) => ReadAssembly(ReferencedAssemblies, References[(reference.FullName, reference.Version)]);
    public static AssemblyDefinition ReadAssembly(string path, ReaderParameters parameters = null) => ReadAssembly(OtherAssemblies, path, parameters);

    private static AssemblyDefinition ReadAssembly(Dictionary<string, AssemblyDefinition> assemblies, string path, ReaderParameters parameters = null) => assemblies.TryGetValue(path, out var assembly) ? 
        assembly : assemblies[path] = AssemblyDefinition.ReadAssembly(path, parameters ?? new() { InMemory = true, ReadWrite = false });

    public static TypeDefinition GetDefinition(Type type) => GetDefinition(type.GetReference());
    public static TypeDefinition GetDefinition(TypeReference type) => GetReferencedAssembly((AssemblyNameReference)type.Scope).MainModule.GetType(type.FullName);

    public static void SetBaseType(TypeDefinition type, TypeDefinition baseType)
    {
        var reference = type.Module.ImportReference(baseType);
        if (type.IsInterface || baseType.IsInterface) type.Interfaces.Add(new(reference));
        else type.BaseType = reference;
    }

    protected override void Visit(AssemblyDefinition _)
    {
        Directory.CreateDirectory(Dir);
        foreach (var entry in ReferencedAssemblies)
        {
            entry.Value.MainModule.Attributes |= ModuleAttributes.ILOnly;
            using (entry.Value) entry.Value.Write(Path.Combine(Dir, Path.GetFileName(entry.Key)));
        }

        foreach (var entry in OtherAssemblies)
        {
            entry.Value.MainModule.Attributes |= ModuleAttributes.ILOnly;
            using (entry.Value) entry.Value.Write(entry.Key);
        }

        ReferencedAssemblies.Clear();
        OtherAssemblies.Clear();
    }

    public override int Priority => int.MaxValue;
}
