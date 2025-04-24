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
    private IFolder SubFolder => _Folder.SubFolder("Sub");
    private IFolder SubFolderTwo => _Folder.SubFolder("SubTwo");
    private string NameOfExistingFile => SubFolder.FullName + @"\i_do_exist.txt";
    private IChangedArborFoldersRepository? _ChangedArborFoldersRepository;
    private FileSystemWatcher? _Watcher;
    private readonly TimeSpan _Delay = TimeSpan.FromMilliseconds(200);

    [TestInitialize]
    public void Initialize() {
        _Folder.CreateIfNecessary();
        SubFolder.CreateIfNecessary();
        SubFolderTwo.CreateIfNecessary();
        foreach(IFolder folder in new List<IFolder> {  _Folder, SubFolder, SubFolderTwo }) {
            foreach (string file in Directory.GetFiles(folder.FullName, "*")) {
                File.Delete(file);
            }
        }
        File.WriteAllText(NameOfExistingFile, NameOfExistingFile);

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
        Assert.AreEqual(0, _ChangedArborFoldersRepository!.ChangedFolders.Count);
        await File.WriteAllTextAsync(_Folder.FullName + @"\new_file.txt", "I am a new file");
        await Task.Delay(_Delay);
        IList<ChangedFolder> changedFolders = _ChangedArborFoldersRepository!.ChangedFolders;
        Assert.AreEqual(1, changedFolders.Count);
        Assert.AreEqual(_Folder.FullName, changedFolders[0].Folder.FullName);
    }

    [TestMethod]
    public async Task DeletedFile_OneChangedFolder() {
        Assert.AreEqual(0, _ChangedArborFoldersRepository!.ChangedFolders.Count);
        File.Delete(NameOfExistingFile);
        await Task.Delay(_Delay);
        IList<ChangedFolder> changedFolders = _ChangedArborFoldersRepository!.ChangedFolders;
        Assert.AreEqual(1, changedFolders.Count);
        Assert.AreEqual(SubFolder.FullName, changedFolders[0].Folder.FullName);
    }

    [TestMethod]
    public async Task MoveFileToAnotherFolder_ThreeChangedFolders() {
        Assert.AreEqual(0, _ChangedArborFoldersRepository!.ChangedFolders.Count);
        File.Move(NameOfExistingFile, NameOfExistingFile.Replace(SubFolder.FullName, SubFolderTwo.FullName));
        await Task.Delay(_Delay);
        IList<ChangedFolder> changedFolders = _ChangedArborFoldersRepository!.ChangedFolders;
        Assert.AreEqual(2, changedFolders.Count);
        Assert.IsTrue(changedFolders.Any(f => f.Folder.FullName == SubFolder.FullName));
        Assert.IsTrue(changedFolders.Any(f => f.Folder.FullName == SubFolderTwo.FullName));
    }
}
