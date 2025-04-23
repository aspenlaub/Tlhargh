using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

[XmlRoot(nameof(ArborFolders), Namespace = "http://www.aspenlaub.net")]
public class ArborFolders : List<ArborFolder>, ISecretResult<ArborFolders> {
    public ArborFolders Clone() {
        var clone = new ArborFolders();
        clone.AddRange(this);
        return clone;
    }
}