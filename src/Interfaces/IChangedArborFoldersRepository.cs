using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface IChangedArborFoldersRepository {
    void SetWorkingFolder(IFolder workingFolder);

    void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder);
    IList<ChangedFolder> FoldersWithChanges();

    event EventHandler<ChangedFolder>? OnChangedFolderAdded;
}
