namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class TlharghProcessorParms {
    public string Command { get; set; } = "";
    public string ArborPathVar { get; set; } = "";
    public string ArborSubFolder { get; set; } = "";

    public Dictionary<string, string> ToDictionary() {
        return new Dictionary<string, string> {
            { nameof(Command), Command },
            { nameof(ArborPathVar), ArborPathVar },
            { nameof(ArborSubFolder), ArborSubFolder }
        };
    }
}
