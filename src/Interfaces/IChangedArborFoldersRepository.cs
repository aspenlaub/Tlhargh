using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface IChangedArborFoldersRepository {
    void SetWorkingFolder(IFolder workingFolder);
    void IncludeTemp();

    void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder);
    void UnregisterChangeInFolder(ChangedFolder folderToUnregister);
    IList<ChangedFolder> FoldersWithChanges();

    event EventHandler<ChangedFolder>? OnChangedFolderAdded;
    event EventHandler<ChangedFolder>? OnChangedFolderRemoved;
    event EventHandler? OnFolderSurvived;
}
