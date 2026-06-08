using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;


public class SearchLocalPackagesHandler
{
    public Task Handle(SearchPackageQuery query, IPackageRepository packageRepository)
    {
        packageRepository.Query()
    }
}
