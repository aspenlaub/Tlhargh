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

    private IFolder _WorkingFolder = new Folder(Path.GetTempPath()).SubFolder("Default" + nameof(ChangedArborFoldersRepository));
    private bool _IncludeTemp;

    public void SetWorkingFolder(IFolder workingFolder) {
        _WorkingFolder = workingFolder;
    }

    public void IncludeTemp() {
        _IncludeTemp = true;
    }

    private readonly List<string> _InfixesToExclude = [
        @"\temp\",
        @"\Temp\",
        @"\files\",
        @"\.vs",
        @"\obj\",
        @"\Obj\",
        @"\debug\",
        @"\Debug\",
        @"\release\",
        @"\Release\",
        @"\bin\",
        @"Bin\",
        @"TestResults\",
        @"Archiv\",
        @"\CSharp\Common\",
        @"\packages\"
    ];

    private bool ShouldBeExcluded(IFolder folder) {
        return !_IncludeTemp
               && _InfixesToExclude.Any(infix
                => (folder.FullName + "\\").Contains(infix, StringComparison.InvariantCulture));
    }

    public void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        if (ShouldBeExcluded(folder)) { return; }

        if (ChangedFolders.Any(f => f.Folder.FullName == folder.FullName)) { return; }

        var changedFolder = new ChangedFolder { ArborFolder = arborFolder, Folder = folder };
        ChangedFolders.Add(changedFolder);
        PersistToFile(changedFolder, false);
        ChangedFolderAdded(changedFolder);
    }

    public void UnregisterChangeInFolder(ChangedFolder folderToUnregister) {
        ChangedFolder? changedFolder = ChangedFolders.FirstOrDefault(f => f.EqualTo(folderToUnregister));
        if (changedFolder != null) {
            ChangedFolders.Remove(changedFolder);
            PersistToFile(changedFolder, true);
            ChangedFolderRemoved(changedFolder);
        } else {
            changedFolder = FoldersWithChanges().FirstOrDefault(f => f.EqualTo(folderToUnregister));
            if (changedFolder != null) {
                PersistToFile(changedFolder, true);
                ChangedFolderRemoved(changedFolder);
            }
        }

        changedFolder = FoldersWithChanges().FirstOrDefault(f => f.EqualTo(folderToUnregister));
        if (changedFolder != null) {
            FolderSurvived();
        }
    }

    public IList<ChangedFolder> FoldersWithChanges() {
        foreach (ChangedFolder changedFolder in ChangedFolders) {
            PersistToFile(changedFolder, false);
        }
        return ReadFile();
    }

    private void PersistToFile(ChangedFolder changedFolder, bool removed) {
        while (!TryPersistToFile(changedFolder, removed)) {
            Thread.Sleep(TimeSpan.FromMilliseconds(2500));
        }
    }

    private bool TryPersistToFile(ChangedFolder changedFolder, bool removed) {
        _WorkingFolder.CreateIfNecessary();
        string fileName = FileName();
        List<ChangedFolder> changedFolders = ReadFile();
        ChangedFolder? changedFolderInFile = changedFolders.FirstOrDefault(f => f.EqualTo(changedFolder));

        if (removed) {
            if (changedFolderInFile == null) { return true; }
            changedFolders.Remove(changedFolderInFile);
        } else {
            if (changedFolderInFile != null) { return true; }
            changedFolders.Add(changedFolder);
        }

        try {
            File.WriteAllText(fileName, JsonSerializer.Serialize(changedFolders));
        } catch {
            return false;
        }

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

    public event EventHandler<ChangedFolder>? OnChangedFolderAdded;

    protected virtual void ChangedFolderAdded(ChangedFolder changedFolder) {
        OnChangedFolderAdded?.Invoke(this, changedFolder);
    }

    public event EventHandler<ChangedFolder>? OnChangedFolderRemoved;

    protected virtual void ChangedFolderRemoved(ChangedFolder changedFolder) {
        OnChangedFolderRemoved?.Invoke(this, changedFolder);
    }

    public event EventHandler? OnFolderSurvived;

    protected virtual void FolderSurvived() {
        OnFolderSurvived?.Invoke(this, EventArgs.Empty);
    }
}
