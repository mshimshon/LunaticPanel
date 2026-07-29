using LunaticPanel.Engine.Plugin.Exceptions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace LunaticPanel.Engine.Plugin;

public static class DotnetInspectorExt
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
        if (Path.GetFileNameWithoutExtension(dll) != "LunaticPanel.PackageManager") return 0;
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);

        if (!pe.HasMetadata)
        {
            Console.WriteLine("[ERROR] File has no metadata structure.");
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' has no metadata.");
        }

        var reader = pe.GetMetadataReader();

        if (!reader.IsAssembly)
        {
            Console.WriteLine("[ERROR] File is not a managed assembly.");
            throw new PluginMetadataExtractionMalformedException("Format", $"'{dll}' is not a managed assembly.");
        }

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

                Console.WriteLine($"       └── [IFACE #{ifaceIndex}] Checking token layout. Kind: {tokenKind}");

                if (tokenKind == HandleKind.TypeReference)
                {
                    var tr = reader.GetTypeReference((TypeReferenceHandle)iface.Interface);
                    string trName = reader.GetString(tr.Name);
                    string trNamespace = reader.GetString(tr.Namespace);

                    Console.WriteLine($"           └── Targets Name: '{trName}', Namespace: '{trNamespace}'");

                    if (comparer.Equals(tr.Name, CORE_ABSTRACT_PLUGIN) && comparer.Equals(tr.Namespace, CORE_ABSTRACT_NAMESPACE))
                    {
                        count++;
                        break;
                    }
                }
            }
        }

        Console.WriteLine($"\n[END] Scan complete. Final count: {count}\n");
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




}
