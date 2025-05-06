using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class TlharghWorker(IChangedArborFoldersRepository repository, ITlharghAccessor accessor) : ITlharghWorker {
    public async Task DoWorkAsync(int counter, DateTime stopAt) {
        do {
            IList<ChangedFolder> changedFolders = repository.FoldersWithChanges();
            if (!changedFolders.Any()) { return; }

            ChangedFolder changedFolder = changedFolders[counter % changedFolders.Count];
            var errorsAndInfos = new ErrorsAndInfos();
            await accessor.ProcessChangedFolderAsync(changedFolder, errorsAndInfos);

            if (errorsAndInfos.AnyErrors()) {
                counter ++;
            } else {
                repository.UnregisterChangeInFolder(changedFolder);
            }
        } while (DateTime.Now < stopAt);
    }
}
