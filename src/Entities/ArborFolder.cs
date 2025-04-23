using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class ArborFolder {
    [XmlAttribute("arborsourcepathvar")]
    public string ArborSourcePathVar { get; set; } = "";

    [XmlAttribute("arbordestpathvar")]
    public string ArborDestPathVar { get; set; } = "";

    [XmlAttribute("fullfolder")]
    public string FullFolder { get; set; } = "";
}
