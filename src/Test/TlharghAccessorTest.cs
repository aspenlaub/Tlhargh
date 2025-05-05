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
public class TlharghAccessorTest {
    private readonly IContainer _Container = new ContainerBuilder().UseTlharghDvinAndPegh().Build();
    private ArborFolder? _ArborFolder;
    private ChangedFolder? _ChangedFolder;

    [TestInitialize]
    public async Task Initialize() {
        IArborFoldersSource foldersSource = _Container.Resolve<IArborFoldersSource>();
        var errorsAndInfos = new ErrorsAndInfos();
        ArborFolders folders = await foldersSource.GetResolvedArborFoldersAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        _ArborFolder = folders.Single(f => f.ArborDestPathVar == "/arboretum/test/");
        IFolder folder = new Folder(_ArborFolder.FullFolder).SubFolder(nameof(TlharghAccessorTest));
        folder.CreateIfNecessary();
        _ChangedFolder = new ChangedFolder { ArborFolder = _ArborFolder, Folder = (Folder)folder };
    }

    [TestMethod]
    public async Task CanProcessChangedFolder() {
        ITlharghAccessor sut = _Container.Resolve<ITlharghAccessor>();
        if (_ChangedFolder == null) {
            throw new NullReferenceException(nameof(_ChangedFolder));
        }
        var errorsAndInfos = new ErrorsAndInfos();
        await sut.ProcessChangedFolderAsync(_ChangedFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
    }

    [TestMethod]
    public async Task CannotProcessChangedFolderWithNonExistingArborPathVar() {
        ITlharghAccessor sut = _Container.Resolve<ITlharghAccessor>();
        IFolder? folder = new Folder(Path.GetTempPath()).SubFolder(nameof(CannotProcessChangedFolderWithNonExistingArborPathVar));
        var changedFolder = new ChangedFolder {
            ArborFolder = new ArborFolder {
                ArborSourcePathVar = "thisArborSourcePathVarDoesNotExist", FullFolder = folder.FullName
            },
            Folder = (Folder)folder
        };
        var errorsAndInfos = new ErrorsAndInfos();
        await sut.ProcessChangedFolderAsync(changedFolder, errorsAndInfos);
        Assert.IsTrue(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.StartsWith("Request could not be processed by")));
    }
}
