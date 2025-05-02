using System.IO;
using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class ChangedArborFoldersRepository(IFolder workingFolder) : IChangedArborFoldersRepository {
    private IList<ChangedFolder> ChangedFolders { get; } = [];

    public void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        if (ChangedFolders.Any(f => f.Folder.FullName == folder.FullName)) { return; }

        var changedFolder = new ChangedFolder { ArborFolder = arborFolder, Folder = folder };
        ChangedFolders.Add(changedFolder);
        PersistToFile(changedFolder);
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
        workingFolder.CreateIfNecessary();
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
        workingFolder.CreateIfNecessary();
        return workingFolder.FullName + $"\\{nameof(ChangedArborFoldersRepository)}.json";
    }

    private List<ChangedFolder> ReadFile() {
        return File.Exists(FileName())
            ? JsonSerializer.Deserialize<List<ChangedFolder>>(File.ReadAllText(FileName())) ?? []
            : [];
    }
}
