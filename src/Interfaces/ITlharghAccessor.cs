using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface ITlharghAccessor {
    Task ProcessChangedFolderAsync(ChangedFolder changedFolder, ErrorsAndInfos errorsAndInfos);
}
