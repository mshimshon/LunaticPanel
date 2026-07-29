using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.PluginValidator.Exceptions;
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
        foreach (var handle in reader.CustomAttributes)
        {
            var attr = reader.GetCustomAttribute(handle);
            var ctor = attr.Constructor;

            if (ctor.Kind != HandleKind.MemberReference)
                continue;

            var memberRef = reader.GetMemberReference((MemberReferenceHandle)ctor);
            var container = memberRef.Parent;

            if (container.Kind != HandleKind.TypeReference)
                continue;

            var typeRef = reader.GetTypeReference((TypeReferenceHandle)container);
            var name = reader.GetString(typeRef.Name);
            var ns = reader.GetString(typeRef.Namespace);

            if (ns != "System.Reflection")
                continue;

            var valueReader = reader.GetBlobReader(attr.Value);

            switch (name)
            {
                case "AssemblyProductAttribute":
                    info[ManifestMeta.Product] = ReadStringArg(valueReader);
                    break;
                case "AssemblyDescriptionAttribute":
                    info[ManifestMeta.Description] = ReadStringArg(valueReader);
                    break;

                case "AssemblyCompanyAttribute":
                    info[ManifestMeta.Company] = ReadStringArg(valueReader);
                    break;

                case "AssemblyTitleAttribute":
                    info[ManifestMeta.Title] = ReadStringArg(valueReader);
                    break;

                case "AssemblyInformationalVersionAttribute":
                    info[ManifestMeta.Version] = ReadStringArg(valueReader);
                    break;

                case "AssemblyFileVersionAttribute":
                    info[ManifestMeta.FileVersion] = ReadStringArg(valueReader);
                    break;
                case "AssemblyCopyrightAttribute":
                    info[ManifestMeta.Copyright] = ReadStringArg(valueReader);
                    break;
            }
        }
        Console.WriteLine("Extracting Metadata Completed");

        return info;
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
        var meta = LibraryValidatorExt.ExtractMetadata(dll);

        var pluginId = meta[ManifestMeta.Id];
        if (pluginId == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Id.ToString(), $"'{dll}' no ID found.");
        var description = meta[ManifestMeta.Description];
        if (description == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Description.ToString(), $"'{dll}' no description found.");
        var company = meta[ManifestMeta.Company];
        if (company == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Company.ToString(), $"'{dll}' company not found.");
        var version = meta[ManifestMeta.Version]?.Split('+')[0];
        if (version == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' version tag not found.");
        string[] versionSplit = version.Split('.');
        if (versionSplit.Length != 3)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' version tag '{version}' doesn't respect strict 'major.minor.patch' format.");

        var asmVersion = meta[ManifestMeta.AssemblyVersion]?.Split('+')[0];
        if (asmVersion == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyVersion tag not found.");
        string[] asmVersionSplit = asmVersion.Split('.');
        if (asmVersionSplit.Length != 4 || asmVersionSplit[3] != "0")
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyVersion tag '{asmVersion}' doesn't respect strict 'major.minor.patch' format.");

        var fileVersion = meta[ManifestMeta.FileVersion]?.Split('+')[0];
        if (fileVersion == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' AssemblyFileVersion tag not found.");
        string[] fileVersionSplit = fileVersion.Split('.');
        if (fileVersionSplit.Length != 4 || fileVersionSplit[3] != "0")
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' AssemblyFileVersion tag '{fileVersion}' doesn't respect strict 'major.minor.patch' format.");

        if (versionSplit[0] != asmVersionSplit[0] || versionSplit[1] != asmVersionSplit[1] || versionSplit[2] != asmVersionSplit[2])
            throw new PluginMetadataExtractionMalformedException("", $"'{dll}' {asmVersion} != {version} Assembly Version must equal Version (without +hash).");
        if (versionSplit[0] != fileVersionSplit[0] || versionSplit[1] != fileVersionSplit[1] || versionSplit[2] != fileVersionSplit[2])
            throw new PluginMetadataExtractionMalformedException("", $"'{dll}' {asmVersion} != {version} AssemblyFileVersion must equal Version (without +hash).");
        int pluginEntryImplementations = CountIPluginImplementations(dll);
        if (pluginEntryImplementations <= 0)
            throw new PluginEntryViolationException("No Plugin Entry Found.");
        if (pluginEntryImplementations > 1)
            throw new PluginEntryViolationException("Only one plugin entry is allowed.");

        foreach (var item in CoreDependencies.OptionalCoreAssemblies)
            MatchReferenceAssemblyVersion(dll, "LunaticPanel.Core.Abstraction", item, false);
        foreach (var item in CoreDependencies.RequiredCoreAssemblies)
            MatchReferenceAssemblyVersion(dll, "LunaticPanel.Core.Abstraction", item, true);

        Version? corePluginVersion = GetReferencedAssemblyVersion(dll, "LunaticPanel.Core.Abstraction");
        if (corePluginVersion == default)
            throw new PluginCoreVersionFailedException("Failed to extract core plugin version.");
        var currentCoreVersion = typeof(IPlugin).Assembly.GetName().Version;
        if (currentCoreVersion == default)
            throw new PluginCoreVersionFailedException("Failed to extract core current version.");
        if (currentCoreVersion.Major != corePluginVersion.Major)
            throw new PluginCoreVersionFailedException($"Plugin was compiled with panel v{corePluginVersion.Major} and the current panel version is v{currentCoreVersion.Major}.");
        if (currentCoreVersion < corePluginVersion)
            throw new PluginCoreVersionFailedException($"Plugin was compiled with panel v{corePluginVersion} and this panel sdk outdated with v{currentCoreVersion}.");


        Console.WriteLine("Validator Success");

    }

}