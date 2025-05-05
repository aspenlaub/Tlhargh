using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class TlharghSettingsSource(ISecretRepository secretRepository) : ITlharghSettingsSource {
    public async Task<TlharghSettings> GetTlharghSettingsAsync(ErrorsAndInfos errorsAndInfos) {
        var secret = new SecretTlharghSettings();
        return await secretRepository.GetAsync(secret, errorsAndInfos);
    }
}
