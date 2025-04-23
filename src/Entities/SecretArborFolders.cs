using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

public class SecretArborFolders : ISecret<ArborFolders> {
    private ArborFolders? _LogicalFolders;
    public ArborFolders DefaultValue => _LogicalFolders ??= [new() {
        ArborSourcePathVar = "arSoPaVa",
        ArborDestPathVar = "arDePaVa",
        FullFolder = @"c:\temp\"
    }];

    public string Guid => "7116011B-E01E-C150-1D79-AD8B0309916B";
}