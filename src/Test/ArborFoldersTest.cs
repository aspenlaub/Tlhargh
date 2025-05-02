using System.Net.Http.Json;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Test;

[TestClass]
public class ArborFoldersTest {
    private IContainer? _PeghContainer, _Container;

    [TestInitialize]
    public void Initialize() {
        _PeghContainer = new ContainerBuilder().UsePegh(nameof(ArborFoldersTest), new DummyCsArgumentPrompter()).Build();
        _Container = new ContainerBuilder().UseTlharghDvinAndPegh().Build();
    }

    [TestMethod]
    public async Task CanGetArborFolders() {
        var secret = new SecretArborFolders();
        var errorsAndInfos = new ErrorsAndInfos();
        ArborFolders arborFolders = await _PeghContainer!.Resolve<ISecretRepository>().GetAsync(secret, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(arborFolders.Count > 0);
    }

    [TestMethod]
    public async Task CanGetTlharghSettings() {
        var secret = new SecretTlharghSettings();
        var errorsAndInfos = new ErrorsAndInfos();
        TlharghSettings settings = await _PeghContainer!.Resolve<ISecretRepository>().GetAsync(secret, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(!string.IsNullOrEmpty(settings.GetArborFoldersUrl));
    }

    [TestMethod]
    public async Task ArborFoldersAreConfiguredCorrectly() {
        var secret = new SecretTlharghSettings();
        var errorsAndInfos = new ErrorsAndInfos();
        TlharghSettings settings = await _PeghContainer!.Resolve<ISecretRepository>().GetAsync(secret, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        var client = new HttpClient();
        Dictionary<string, ArborFolder> expectedArborFolders = (await client.GetFromJsonAsync<List<ArborFolder>>(settings.GetArborFoldersUrl))?
               .ToDictionary(x => x.ArborSourcePathVar, x => x)
               ?? [];
        ArborFolders arborFolders = await _Container!.Resolve<IArborFoldersSource>().GetResolvedArborFoldersAsync(errorsAndInfos);
        var actualArborFolders = arborFolders
             .ToDictionary(x => x.ArborSourcePathVar, x => x);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        foreach (string arborSourcePathVar in expectedArborFolders.Keys) {
            Assert.IsTrue(actualArborFolders.ContainsKey(arborSourcePathVar), $"Missing ArborSourcePathVar '{arborSourcePathVar}'");
            Assert.AreEqual(expectedArborFolders[arborSourcePathVar].ArborDestPathVar,
                actualArborFolders[arborSourcePathVar].ArborDestPathVar,
                $"ArborDestPathVar for '{arborSourcePathVar}' should be {expectedArborFolders[arborSourcePathVar].ArborDestPathVar}");
            Assert.AreEqual(expectedArborFolders[arborSourcePathVar].FullFolder,
                actualArborFolders[arborSourcePathVar].FullFolder,
                $"FullName for '{arborSourcePathVar}' should be {expectedArborFolders[arborSourcePathVar].FullFolder}");
        }
    }
}
