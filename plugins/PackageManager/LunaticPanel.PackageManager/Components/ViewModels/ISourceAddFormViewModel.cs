using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using MudBlazor;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface ISourceAddFormViewModel : IWidgetViewModel
{
    IMudDialogInstance MudDialog { get; set; }
    string SourceName { get; set; }
    string SourceLocation { get; set; }
    RepositorySourceType SourceType { get; set; }
    Func<string, string> OnValidateSourceName { get; }
    Func<string, string> OnValidateSourceLocation { get; }
    Task Create();

}
