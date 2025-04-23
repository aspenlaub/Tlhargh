using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class ChangedArborFoldersRepository : IChangedArborFoldersRepository {
    public IList<ChangedFolder> ChangedFolders { get; } = new List<ChangedFolder>();

    public void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder) {
        if (ChangedFolders.Any(f => f.Folder.FullName == folder.FullName)) { return; }

        ChangedFolders.Add(new ChangedFolder {
            ArborFolder = arborFolder, Folder = folder
        });
    }
}
