using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Test;

public class FakeChangedArborFoldersRepository : IChangedArborFoldersRepository {
    private IList<ChangedFolder> ChangedFolders { get; set; } = [];

    public void SetWorkingFolder(IFolder workingFolder) {
    }

    public void Reset() {
        ChangedFolders = [];
    }

    public void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        var changedFolder = new ChangedFolder { ArborFolder = arborFolder, Folder = folder };
        ChangedFolders.Add(changedFolder);
        ChangedFolderAdded(changedFolder);
    }

    public void UnregisterChangeInFolder(ChangedFolder folderToUnregister) {
        ChangedFolder? changedFolder = ChangedFolders.FirstOrDefault(f => f.EqualTo(folderToUnregister));
        if (changedFolder == null) { return; }

        ChangedFolders.Remove(changedFolder);
        ChangedFolderRemoved(changedFolder);
    }

    public IList<ChangedFolder> FoldersWithChanges() {
        return ChangedFolders;
    }

    public event EventHandler<ChangedFolder>? OnChangedFolderAdded;

    protected virtual void ChangedFolderAdded(ChangedFolder changedFolder) {
        OnChangedFolderAdded?.Invoke(this, changedFolder);
    }

    public event EventHandler<ChangedFolder>? OnChangedFolderRemoved;

    protected virtual void ChangedFolderRemoved(ChangedFolder changedFolder) {
        OnChangedFolderRemoved?.Invoke(this, changedFolder);
    }
}
