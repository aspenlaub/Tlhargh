using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface IChangedArborFoldersRepository {
    IList<ChangedFolder> ChangedFolders { get; }

    void RegisterChangeInFolder(ArborFolder arborFolder, Folder folder);
}
