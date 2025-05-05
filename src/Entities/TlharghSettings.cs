using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class TlharghSettings : ISecretResult<TlharghSettings> {
    [XmlAttribute("getarborfoldersurl")]
    public string GetArborFoldersUrl { get; set; } = "";

    [XmlAttribute("tlharghbackendurl")]
    public string TlharghBackendUrl { get; set; } = "";

    public TlharghSettings Clone() {
        return new TlharghSettings {
            GetArborFoldersUrl = GetArborFoldersUrl,
            TlharghBackendUrl = TlharghBackendUrl
        };
    }
}
