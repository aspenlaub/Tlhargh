using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class SecretTlharghSettings : ISecret<TlharghSettings> {
    private static TlharghSettings? _defaultTlharghSettings;
    public TlharghSettings DefaultValue => _defaultTlharghSettings ??= new TlharghSettings { GetArborFoldersUrl = "http://localhost" };

    public string Guid => "661DA66C-E118-39BC-CC02-BFCD2DD4AB8E";
}