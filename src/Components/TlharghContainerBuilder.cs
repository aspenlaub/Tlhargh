using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public static class TlharghContainerBuilder {
    // ReSharper disable once UnusedMember.Global
    public static ContainerBuilder RegisterForTlharghDvinAndPegh(this ContainerBuilder builder, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        builder.UseDvinAndPegh(applicationName, csArgumentPrompter);
        return builder;
    }

    public static IServiceCollection UseTlharghDvinAndPegh(this IServiceCollection services, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        services.UseDvinAndPegh(applicationName, csArgumentPrompter);
        return services;
    }
}
