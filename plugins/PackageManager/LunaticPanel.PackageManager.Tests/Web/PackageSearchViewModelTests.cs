using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mock;
using StatePulse.Net;
using System;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using LunaticPanel.PackageManager.Application.Payloads;
using NSubstitute;
using NSubstitute.Extensions;
using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Components.ViewModels;
namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Web;

public class PackageSearchViewModelTests
{
    private readonly IStatePulse _statePulse;
    private static PackageManifestPayload CreatePackageManifest(
        string id,
        string title,
        string version,
        string panelVersion) => new()
        {
            Id = id,
            Title = title,
            Company = "Contoso",
            Version = version,
            Description = "Package test description.",
            Author = "Whatever",
            Copyright = "© Contoso",
            PanelVersion = panelVersion,
            DotnetVersion = "8.0.12",
            PluginEntryFile = "Package.dll"
        };
    private static PackagePayload CreatePackage(
        string id,
        string title,
        string version,
        string panelVersion) => new()
        {
            Info = new()
            {
                PackageId = id,
                Description = "Package test description.",
                Name = title,
            },
            AutoUpdate = true,
            Version = version,
            PanelVersion = panelVersion
        };
    private readonly PackageSearchViewModel _handler;
    public PackageSearchViewModelTests()
    {
        _statePulse = Substitute.For<IStatePulse>();
        // _handler = new PackageSearchViewModel(_statePulse);
    }

}
