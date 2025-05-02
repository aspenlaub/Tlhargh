using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public static class TlharghContainerBuilder {
    // ReSharper disable once UnusedMember.Global
    public static ContainerBuilder UseTlharghDvinAndPegh(this ContainerBuilder builder) {
        builder.UsePegh(Constants.TlharghAppId, new DummyCsArgumentPrompter());
        builder.RegisterType<ArborFoldersSource>().As<IArborFoldersSource>().SingleInstance();
        builder.RegisterType<ChangedArborFoldersRepository>().As<IChangedArborFoldersRepository>().SingleInstance();

        return builder;
    }
}
