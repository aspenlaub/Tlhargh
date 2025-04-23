using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class ArborFoldersSource(ISecretRepository secretRepository, IFolderResolver folderResolver) : IArborFoldersSource {
    public async Task<ArborFolders> GetResolvedArborFoldersAsync(IErrorsAndInfos errorsAndInfos) {
        var secret = new SecretArborFolders();
        ArborFolders? arborFolders = await secretRepository.GetAsync(secret, errorsAndInfos);
        foreach(ArborFolder arborFolder in arborFolders) {
            IFolder? resolvedFolder = await folderResolver.ResolveAsync(arborFolder.FullFolder, errorsAndInfos);
            arborFolder.FullFolder = resolvedFolder.FullName + '\\';
        }

        return arborFolders;
    }
}
