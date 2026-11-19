using LunaticPanel.Core.Utils.Abstraction.IO;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Repositories;
using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Infrastructure;

public class SourceRepositoryTests
{
    private readonly ICrazyReport<SourceRepository> _crazyReport;
    private readonly IAtomicFileSystem _atomicFileSystem;
    private readonly IPluginLocation _pluginLocation;
    private readonly ISourceFileService _sourceFileService;
    private readonly ISourceRepository _service;
    private readonly string _sourceFile;
    public SourceRepositoryTests() : base()
    {
        _crazyReport = Substitute.For<ICrazyReport<SourceRepository>>();
        _atomicFileSystem = Substitute.For<IAtomicFileSystem>();
        _pluginLocation = Substitute.For<IPluginLocation>();
        _sourceFileService = Substitute.For<ISourceFileService>();
        _sourceFile = Path.GetTempFileName();
        _pluginLocation.GetConfigFor(Arg.Any<string>(), Arg.Any<string>()).Returns(_sourceFile);
        _service = new SourceRepository(_atomicFileSystem, _pluginLocation, _sourceFileService);
    }

    private readonly ICollection<RepositorySourceEntity> _packageSources =
      [
            PackageExt.CreateRepositorySourceEntity("Source A", "http://sss.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Enabled),
      ];

    [Fact]
    public async Task SourceRepositoryAdd_ShouldPass()
    {
        await _service.AddAsync(_packageSources.First(), TestContext.Current.CancellationToken);
        await _atomicFileSystem.Received().WriteAtomicAsync(Arg.Any<string>(), Arg.Any<Func<string, string>>(), Arg.Any<CancellationToken>(), Arg.Any<string?>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task SourceRepositoryDisableAsync_ShouldPass()
    {
        await _service.DisableAsync(_packageSources.First(), TestContext.Current.CancellationToken);
        await _atomicFileSystem.Received().WriteAtomicAsync(Arg.Any<string>(), Arg.Any<Func<string, string>>(), Arg.Any<CancellationToken>(), Arg.Any<string?>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task SourceRepositoryEnableAsync_ShouldPass()
    {
        await _service.EnableAsync(_packageSources.First(), TestContext.Current.CancellationToken);
        await _atomicFileSystem.Received().WriteAtomicAsync(Arg.Any<string>(), Arg.Any<Func<string, string>>(), Arg.Any<CancellationToken>(), Arg.Any<string?>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task SourceRepositoryGetAllAsync_ShouldPass()
    {

        var result = await _service.GetAllAsync(TestContext.Current.CancellationToken);
        Assert.True(result.Count() == 0);
    }

    [Fact]
    public async Task SourceRepositoryRemoveAsync_ShouldPass()
    {
        await _service.RemoveAsync(_packageSources.First(), TestContext.Current.CancellationToken);
        await _atomicFileSystem.Received().WriteAtomicAsync(Arg.Any<string>(), Arg.Any<Func<string, string>>(), Arg.Any<CancellationToken>(), Arg.Any<string?>(), Arg.Any<string?>());
    }



}
