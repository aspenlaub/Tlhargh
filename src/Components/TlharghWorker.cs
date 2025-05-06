using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class TlharghWorker(IChangedArborFoldersRepository repository) : ITlharghWorker {
    public void DoWork(int counter, DateTime stopAt) {
        do {
            IList<ChangedFolder> changedFolders = repository.FoldersWithChanges();
            if (!changedFolders.Any()) { return; }

            ChangedFolder changedFolder = changedFolders[counter % changedFolders.Count];
            repository.UnregisterChangeInFolder(changedFolder.ArborFolder, changedFolder.Folder);
        } while (DateTime.Now < stopAt);
    }
}
