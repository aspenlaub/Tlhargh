using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Test;

[TestClass]
public class ArborFolderWatcherTest {
    private readonly IFolder _Folder = new Folder(Path.GetTempPath()).SubFolder(nameof(ArborFolderWatcherTest));
    private IChangedArborFoldersRepository? _ChangedArborFoldersRepository;
    private FileSystemWatcher? _Watcher;

    [TestInitialize]
    public void Initialize() {
        _Folder.CreateIfNecessary();
        foreach (string file in Directory.GetFiles(_Folder.FullName, "*")) {
            File.Delete(file);
        }

        _ChangedArborFoldersRepository = new ChangedArborFoldersRepository();
        var arborFolder = new ArborFolder { FullFolder = _Folder.FullName + '\\' };
        _Watcher = new ArborFolderWatcherFactory(_ChangedArborFoldersRepository, arborFolder).Create();
    }

    [TestCleanup]
    public void Cleanup() {
        _Watcher!.Dispose();
    }

    [TestMethod]
    public void NoFileChanges_NoChangedFolders() {
        IList<ChangedFolder> changedFolders = _ChangedArborFoldersRepository!.ChangedFolders;
        Assert.AreEqual(0, changedFolders.Count);
    }

    [TestMethod]
    public async Task NewFile_OneChangedFolder() {
        await File.WriteAllTextAsync(_Folder.FullName + @"\new_file.txt", "I am a new file");
        IList<ChangedFolder> changedFolders = _ChangedArborFoldersRepository!.ChangedFolders;
        Assert.AreEqual(1, changedFolders.Count);
        Assert.AreEqual(_Folder.FullName, changedFolders[0].Folder.FullName);
    }
}
