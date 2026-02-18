using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface ITlharghSettingsSource {
    Task<TlharghSettings> GetTlharghSettingsAsync(ErrorsAndInfos errorsAndInfos);
}
