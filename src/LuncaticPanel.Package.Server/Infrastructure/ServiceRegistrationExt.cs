using LunaticPanel.Core.Utils;
using LuncaticPanel.Package.Server.Application;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Infrastructure;

public static class ServiceRegistrationExt
{
    internal static void AddInfrastructureLayerServices(this IServiceCollection services)
    {
        services.AddApplicationLayerServices();
        services.AddTransient<IPackageValidatorService, PackageValidationService>();
        services.AddLinuxCommandUtilityService();
    }
}
/*
 * Endpoint -> Mediator -> Domain Repo ->
 * 1. Endpoints must be defined and accessible at all times and follow the LPKG 1.0
 * 2. The Flow cannot be changed or interrupted at any point, it must go through the steps.
 * 3. A Repos must be implemented by the consuming project of the package
 * 4. Any Pull of information is design to be public to some capacity and is supplied the information.
 * 5. The API is deisgn to provide information and not to update or push information directly.
 * The goal of package is to supply hard locked process design such as standarize fetch of information and standarize processing of package validation.
 * -> Get = /lpkg/v1/package/search - this return a paginated search result.
 * -> Get = /lpkg/v1/package/info/PKGID - this return all of the manifest for all available versions.
 * -> Get = /lpkg/v1/package/info/PKGID/1.0.0 - this return the package manifest for the specific version requested.
 * -> Get = /lpkg/versions - This return all of the supported API Versions.
 * -> Following Must be enabled manually and secured behind security measures
 * -> Post = /lpkg/v1/package/push 
 * -> This is design to trigger the test workflow, we get a URL of package where it was upload and download it,we check the lpkg version and get the corresponding testing tool and test it and if validate passes we proceed to adding otherwise we reject with an event or a failure entry of some sort
 * -> This is design to be called when testing is ready in theory the upload feature fully decoupled and in best practice should trigger a queued workflow and the queue will eventually call this endpoint to perform package validation.
 * -> That's it i think anything else such ownership or transfer of such should be handle by another domain our package ownership is irrelevent for this domain. 
 
 */