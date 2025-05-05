using System.Text.Json.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class ChangedFolder {
    public ArborFolder ArborFolder { get; init; } = new();
    public Folder Folder { get; init; } = new("");
    [JsonIgnore]
    public string ArborSubFolder => Folder.FullName.Replace(ArborFolder.FullFolder, "").Replace("\\", "/");

    public bool EqualTo(ChangedFolder changedFolder) {
        return Folder.FullName == changedFolder.Folder.FullName
               && ArborFolder.ArborSourcePathVar == changedFolder.ArborFolder.ArborSourcePathVar;
    }

    public override string ToString() {
        return ArborFolder.ArborDestPathVar + ArborSubFolder;
    }
}
