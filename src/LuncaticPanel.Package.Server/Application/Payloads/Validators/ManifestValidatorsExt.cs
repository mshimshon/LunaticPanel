namespace LuncaticPanel.Package.Server.Application.Payloads.Validators;

public static class ManifestValidatorsExt
{

    public static bool ValidPackageId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        return true;
    }
}
