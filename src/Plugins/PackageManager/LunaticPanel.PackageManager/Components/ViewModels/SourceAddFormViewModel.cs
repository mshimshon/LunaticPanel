using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.Exceptions;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using MudBlazor;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceAddFormViewModel : WidgetViewModelBase, ISourceAddFormViewModel
{
    public SourceAddFormViewModel()
    {

    }

    public IMudDialogInstance MudDialog { get; set; } = default!;
    public string SourceName { get; set; } = default!;
    public string SourceLocation { get; set; } = default!;
    public RepositorySourceType SourceType { get; set; } = default!;

    public Func<string, string> OnValidateSourceName => ValidateSourceName!;

    public Func<string, string> OnValidateSourceLocation => ValidateSourceLocation!;

    public async Task Create()
    {
        RepositorySource sourceEntity = SourceType == RepositorySourceType.Local ? new RepositorySourceLocal(SourceLocation) : new RepositorySourceRemote(SourceLocation);
        RepositorySourceName sourceName = new RepositorySourceName(SourceName);
        RepositorySourceInfo sourceInfo = new RepositorySourceInfo(sourceEntity, SourceType);
        RepositorySourceEntity entityResult = new RepositorySourceEntity(sourceName, sourceInfo, RepositorySourceState.Enabled);
        var returnValue = entityResult.ToApplicationPayload();
        MudDialog.Close(returnValue);
    }

    private string? ValidateSourceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Required field display name";
        try
        {
            new RepositorySourceName(value);
        }
        catch (Exception)
        {
            return "Unknown Exception.";
        }
        return null;
    }

    private string? ValidateSourceLocation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Required field either https://apienpoint or /etc/local/on/disk";
        try
        {
            new RepositorySourceRemote(value);
            SourceType = RepositorySourceType.Remote;
        }
        catch (RepositorySourceRemoteNotUrlException)
        {
            if (!Directory.Exists(value))
                return "Location is not a valid URL nor a valid path.";
            SourceType = RepositorySourceType.Local;
        }
        catch (Exception)
        {
            return "Unknown Exception.";
        }
        return null;
    }
}
