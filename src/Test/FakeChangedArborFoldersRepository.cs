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
        ChangedFolders.Add(new ChangedFolder { ArborFolder = arborFolder, Folder = folder });
    }

    public void UnregisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        ChangedFolder? changedFolder = ChangedFolders.FirstOrDefault(f => f.Folder.FullName == folder.FullName);
        if (changedFolder == null) { return; }

        ChangedFolders.Remove(changedFolder);
    }

    public IList<ChangedFolder> FoldersWithChanges() {
        return ChangedFolders;
    }

    public event EventHandler<ChangedFolder>? OnChangedFolderAdded;
    public event EventHandler<ChangedFolder>? OnChangedFolderRemoved;
}
