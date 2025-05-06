using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Test;

public class FakeTlharghAccessor : ITlharghAccessor {
    private bool _LetNextProcessingFail;

    public void Reset() {
        _LetNextProcessingFail = false;
    }

    public void LetNextProcessingFail() {
        _LetNextProcessingFail = true;
    }

    public async Task ProcessChangedFolderAsync(ChangedFolder changedFolder, ErrorsAndInfos errorsAndInfos) {
        if (!_LetNextProcessingFail) { return; }

        errorsAndInfos.Errors.Add("An error occurred");
        _LetNextProcessingFail = false;
        await Task.CompletedTask;
    }
}
