using System.IO;
using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class ChangedArborFoldersRepository : IChangedArborFoldersRepository {
    private IList<ChangedFolder> ChangedFolders { get; } = [];

    private IFolder _WorkingFolder = (new Folder(Path.GetTempPath())).SubFolder("Default" + nameof(ChangedArborFoldersRepository));

    public void SetWorkingFolder(IFolder workingFolder) {
        _WorkingFolder = workingFolder;
    }

    public void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        if (ChangedFolders.Any(f => f.Folder.FullName == folder.FullName)) { return; }

        var changedFolder = new ChangedFolder { ArborFolder = arborFolder, Folder = folder };
        ChangedFolders.Add(changedFolder);
        PersistToFile(changedFolder);
        ChangedFolderAdded(changedFolder.ArborFolder.ArborDestPathVar
            + changedFolder.Folder.FullName.Replace(arborFolder.FullFolder, "").Replace("\\", "/"));
    }

    public IList<ChangedFolder> FoldersWithChanges() {
        foreach (ChangedFolder changedFolder in ChangedFolders) {
            PersistToFile(changedFolder);
        }
        return ReadFile();
    }

    private void PersistToFile(ChangedFolder changedFolder) {
        while (!TryPersistToFile(changedFolder)) {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }

    private bool TryPersistToFile(ChangedFolder changedFolder) {
        _WorkingFolder.CreateIfNecessary();
        string fileName = FileName();
        List<ChangedFolder> changedFolders = ReadFile();
        if (changedFolders.Any(f => f.EqualTo(changedFolder))) { return true; }

        changedFolders.Add(changedFolder);
        File.WriteAllText(fileName, JsonSerializer.Serialize(changedFolders));
        List<ChangedFolder> changedFoldersCheck = ReadFile();
        if (changedFolders.All(f => changedFoldersCheck.Any(fc => fc.EqualTo(f)))) {
            return true;
        }

        foreach (ChangedFolder changedFolderToSurvive in
                 changedFolders.Where(
                     changedFolderToSurvive => !ChangedFolders.Any(f => f.EqualTo(changedFolderToSurvive))
                 )) {
            ChangedFolders.Add(changedFolderToSurvive);
        }

        return false;
    }

    private string FileName() {
        _WorkingFolder.CreateIfNecessary();
        return _WorkingFolder.FullName + $"\\{nameof(ChangedArborFoldersRepository)}.json";
    }

    private List<ChangedFolder> ReadFile() {
        return File.Exists(FileName())
            ? JsonSerializer.Deserialize<List<ChangedFolder>>(File.ReadAllText(FileName())) ?? []
            : [];
    }

    public event EventHandler<string>? OnChangedFolderAdded;

    protected virtual void ChangedFolderAdded(string changedFolder) {
        OnChangedFolderAdded?.Invoke(this, changedFolder);
    }
}
