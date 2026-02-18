using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface IArborFoldersSource {
    Task<ArborFolders> GetResolvedArborFoldersAsync(IErrorsAndInfos errorsAndInfos);
}
