using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.PluginValidator.Exceptions;
using NuGet.Versioning;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace LunaticPanel.Core.PluginValidator;


public static class LibraryValidatorExt
{
    private const string CORE_ABSTRACT_PLUGIN = "IPlugin";
    private const string CORE_ABSTRACT_ROOT_NAMESPACE = "LunaticPanel.Core.Abstraction";
    private const string CORE_ABSTRACT_NAMESPACE = $"{CORE_ABSTRACT_ROOT_NAMESPACE}.Plugin";
    private static string? ReadStringArg(BlobReader reader)
    {
        // Check if the blob is empty or too short for a prolog
        if (reader.Length < 2)
            return null;

        // Check if the valid ECMA-335 custom attribute prolog (0x0001) is present
        ushort prolog = reader.ReadUInt16();
        if (prolog != 1)
        {
            // If it doesn't match the prolog format, reset or return null safely
            return null;
        }

        // Safely check if there is string content left to read
        if (reader.RemainingBytes == 0)
            return null;

        return reader.ReadSerializedString();
    }



    private static KeyValuePair<ManifestMeta?, string?> GetMeta(MetadataReader reader, CustomAttribute customAttribute)
    {
        var container = reader.GetMemberReference((MemberReferenceHandle)customAttribute.Constructor).Parent;
        var typeRef = reader.GetTypeReference((TypeReferenceHandle)container);
        var name = reader.GetString(typeRef.Name);
        var info = new Dictionary<ManifestMeta, string?>();
        var valueReader = reader.GetBlobReader(customAttribute.Value);
        var value = ReadStringArg(valueReader);
        // System.Console.WriteLine($"{name}={value}");
        switch (name)
        {
            case "AssemblyProductAttribute":
                return new(ManifestMeta.Product, value);
            case "AssemblyDescriptionAttribute":
                return new(ManifestMeta.Description, value);
            case "AssemblyCompanyAttribute":
                return new(ManifestMeta.Company, value);
            case "AssemblyTitleAttribute":
                return new(ManifestMeta.Title, value);
            case "AssemblyInformationalVersionAttribute":
                return new(ManifestMeta.Version, value);
            case "AssemblyFileVersionAttribute":
                return new(ManifestMeta.FileVersion, value);
            case "AssemblyCopyrightAttribute":
                return new(ManifestMeta.Copyright, value);
        }
        return new(null, null);
    }
    public static Dictionary<ManifestMeta, string?> ExtractMetadata(string dll)
    {
        Console.WriteLine("Extracting Metadata");

        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");

        var reader = pe.GetMetadataReader();

        if (!reader.IsAssembly)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");

        var info = new Dictionary<ManifestMeta, string?>();
        foreach (ManifestMeta metaKey in Enum.GetValues(typeof(ManifestMeta)))
            info[metaKey] = null;
        var asmDef = reader.GetAssemblyDefinition();
        var asmVersion = asmDef.Version;
        var asmName = reader.GetString(asmDef.Name);
        info[ManifestMeta.Id] = asmName;
        info[ManifestMeta.AssemblyVersion] = asmVersion.ToString();
        var attrMeta = reader.CustomAttributes
            .Select(reader.GetCustomAttribute)
            .Where(p => p.Constructor.Kind == HandleKind.MemberReference).Where(p =>
            {
                var memberRef = reader.GetMemberReference((MemberReferenceHandle)p.Constructor);
                var container = memberRef.Parent;
                return container.Kind == HandleKind.TypeReference;
            }).Where(p =>
            {
                var container = reader.GetMemberReference((MemberReferenceHandle)p.Constructor).Parent;
                var typeRef = reader.GetTypeReference((TypeReferenceHandle)container);
                var ns = reader.GetString(typeRef.Namespace);
                return ns == "System.Reflection";
            })
            .Select(p => GetMeta(reader, p))
            .Where(p => p.Key != null)
            .ToDictionary(p => p.Key!.Value, p => p.Value);
        foreach (var item in attrMeta)
            info[item.Key] = item.Value;
        Console.WriteLine("Extracting Metadata Completed");

        return info;
    }

    public static IEnumerable<(string Name, Version Version)> ReadReferences(string path)
    {
        using var stream = File.OpenRead(path);
        using var pe = new PEReader(stream);
        var reader = pe.GetMetadataReader();

        foreach (var handle in reader.AssemblyReferences)
        {
            var reference = reader.GetAssemblyReference(handle);
            var name = reader.GetString(reference.Name);
            var version = reference.Version;

            yield return (name, version);
        }
    }

    public static bool ContainsIPlugin(string dll)
     => CountIPluginImplementations(dll) > 0;
    public static int CountIPluginImplementations(string dll)
    {
        Console.WriteLine($"[START] Scanning file: '{dll}'");
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");
        var reader = pe.GetMetadataReader();
        if (!reader.IsAssembly)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");


        int count = 0;
        var comparer = reader.StringComparer;

        var asmDefinition = reader.GetAssemblyDefinition();
        Console.WriteLine($"[INFO] Metadata Assembly Name: {reader.GetString(asmDefinition.Name)}");

        int typeIndex = 0;
        foreach (var typeHandle in reader.TypeDefinitions)
        {
            typeIndex++;
            var typeDef = reader.GetTypeDefinition(typeHandle);
            string typeName = reader.GetString(typeDef.Name);
            string typeNamespace = reader.GetString(typeDef.Namespace);
            if (comparer.Equals(typeDef.Name, "<Module>"))
                continue;
            if ((typeDef.Attributes & TypeAttributes.Interface) != 0)
                continue;
            if ((typeDef.Attributes & TypeAttributes.Abstract) != 0)
                continue;


            var interfaces = typeDef.GetInterfaceImplementations();
            int ifaceIndex = 0;
            foreach (var ifaceHandle in interfaces)
            {
                ifaceIndex++;
                var iface = reader.GetInterfaceImplementation(ifaceHandle);
                var tokenKind = iface.Interface.Kind;
                if (tokenKind == HandleKind.TypeReference)
                {
                    var tr = reader.GetTypeReference((TypeReferenceHandle)iface.Interface);
                    string trName = reader.GetString(tr.Name);
                    string trNamespace = reader.GetString(tr.Namespace);
                    if (comparer.Equals(tr.Name, CORE_ABSTRACT_PLUGIN) && comparer.Equals(tr.Namespace, CORE_ABSTRACT_NAMESPACE))
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        return count;
    }

    public static string GetEntryClass(string dll)
    {
        Console.WriteLine($"[START] Scanning file: '{dll}'");
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");
        var reader = pe.GetMetadataReader();
        if (!reader.IsAssembly)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");


        var comparer = reader.StringComparer;
        var asmDefinition = reader.GetAssemblyDefinition();
        Console.WriteLine($"[INFO] Metadata Assembly Name: {reader.GetString(asmDefinition.Name)}");
        int typeIndex = 0;
        foreach (var typeHandle in reader.TypeDefinitions)
        {
            typeIndex++;
            var typeDef = reader.GetTypeDefinition(typeHandle);
            string typeName = reader.GetString(typeDef.Name);
            string typeNamespace = reader.GetString(typeDef.Namespace);
            if (comparer.Equals(typeDef.Name, "<Module>"))
                continue;
            if ((typeDef.Attributes & TypeAttributes.Interface) != 0)
                continue;
            if ((typeDef.Attributes & TypeAttributes.Abstract) != 0)
                continue;


            var interfaces = typeDef.GetInterfaceImplementations();
            int ifaceIndex = 0;
            foreach (var ifaceHandle in interfaces)
            {
                ifaceIndex++;
                var iface = reader.GetInterfaceImplementation(ifaceHandle);
                var tokenKind = iface.Interface.Kind;
                if (tokenKind == HandleKind.TypeReference)
                {
                    var tr = reader.GetTypeReference((TypeReferenceHandle)iface.Interface);
                    string trName = reader.GetString(tr.Name);
                    string trNamespace = reader.GetString(tr.Namespace);
                    if (comparer.Equals(tr.Name, CORE_ABSTRACT_PLUGIN) && comparer.Equals(tr.Namespace, CORE_ABSTRACT_NAMESPACE))
                    {
                        string className = reader.GetString(typeDef.Name);
                        string namespaceName = reader.GetString(typeDef.Namespace);

                        string fullTypeName = string.IsNullOrEmpty(namespaceName)
                            ? className
                            : $"{namespaceName}.{className}";

                        Console.WriteLine($"Found plugin: {fullTypeName}");

                        return fullTypeName;
                    }
                }
            }
        }
        throw new Exception($"Entry Point not Found. {dll}");
    }
    public static Version ReadAssemblyVersion(string dll)
    {
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");

        var reader = pe.GetMetadataReader();

        if (!reader.IsAssembly)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");
        var def = reader.GetAssemblyDefinition();
        return def.Version;
    }
    public static Version? GetReferencedAssemblyVersion(string dll, string assembly)
    {
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");

        var reader = pe.GetMetadataReader();

        if (!reader.IsAssembly)
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");

        foreach (var handle in reader.AssemblyReferences)
        {

            var reference = reader.GetAssemblyReference(handle);
            var name = reader.GetString(reference.Name);

            if (name == assembly)
            {
                return reference.Version;
            }
        }

        return null;
    }

    public static bool IsPluginDllValid(string dll)
    {
        try
        {
            RunPluginDllValidator(dll);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    private static void MatchReferenceAssemblyVersion(string dll, string master, string match, bool throwOnNotFound)
    {
        Version? coreVersionMaster = GetReferencedAssemblyVersion(dll, master);
        if (coreVersionMaster == default)
            throw new HostCodedException("MasterReferenceNotFound", $"{master} not a referenced assembly.");
        Version? corePluginVersion = GetReferencedAssemblyVersion(dll, match);
        if (corePluginVersion == default)
        {

            Console.WriteLine($"Plugin does not have references to {match}.");
            if (throwOnNotFound)
                throw new HostCodedException("CoreReferenceNotFound", $"{match} not a referenced assembly but required.");
            else return;
        }
        if (coreVersionMaster != corePluginVersion)
            throw new HostCodedException("CoreReferenceVersionMisMatch", $"{master} v{coreVersionMaster} and {match} v{corePluginVersion} different versions.");

        Console.WriteLine($"{master} v{coreVersionMaster} and {match} v{corePluginVersion} version aligned.");
    }

    public static void RunPluginDllValidator(string dll)
    {
        Console.WriteLine("RunPluginDllValidator");
        var meta = ExtractMetadata(dll);

        var pluginId = meta[ManifestMeta.Id];
        if (pluginId == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Id.ToString(), $"'{dll}' no ID found.");
        var description = meta[ManifestMeta.Description];
        if (description == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Description.ToString(), $"'{dll}' no description found.");
        var company = meta[ManifestMeta.Company];
        if (company == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Company.ToString(), $"'{dll}' company not found.");

        string? version = meta[ManifestMeta.Version] ?? throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' version tag not found.");
        var tVersion = NuGetVersion.Parse(version);
        if (tVersion.Revision > 0)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' revision version is unsupported.");

        var asmVersion = meta[ManifestMeta.AssemblyVersion] ?? throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyVersion tag not found."); ;
        var tAsmVersion = NuGetVersion.Parse(asmVersion);
        if (tAsmVersion.Major != tVersion.Major || tAsmVersion.Revision > 0 || tAsmVersion.Minor != 0 || tAsmVersion.Patch != 0)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyFileVersion tag '{tAsmVersion.Major}.0.0.0' is not same as '{tVersion.Major}.0.0.0'.");

        var fileVersion = meta[ManifestMeta.FileVersion] ?? throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' FileVersion tag not found.");
        var tFileVersion = NuGetVersion.Parse(fileVersion);
        if (tFileVersion.Version != tVersion.Version)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' FileVersion tag '{tFileVersion.Version}' must be equal to '{tVersion.Version}'.");


        int pluginEntryImplementations = CountIPluginImplementations(dll);
        if (pluginEntryImplementations <= 0)
            throw new PluginEntryViolationException("No Plugin Entry Found.");
        if (pluginEntryImplementations > 1)
            throw new PluginEntryViolationException("Only one plugin entry is allowed.");

        foreach (var item in CoreDependencies.OptionalCoreAssemblies)
            MatchReferenceAssemblyVersion(dll, "LunaticPanel.Core.Abstraction", item, false);
        foreach (var item in CoreDependencies.RequiredCoreAssemblies)
            MatchReferenceAssemblyVersion(dll, "LunaticPanel.Core.Abstraction", item, true);


        Console.WriteLine("Validator Success");

    }

}
