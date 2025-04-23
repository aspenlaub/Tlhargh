using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh;

public static class TlharghModelBuilder {
    public static IEdmModel GetEdmModel() {
        var builder = new ODataConventionModelBuilder {
            Namespace = "Aspenlaub.Net.GitHub.CSharp.Tlhargh",
            ContainerName = "DefaultContainer"
        };
        // builder.EntitySet<ControllableProcess>("ControllableProcesses");
        // builder.EntitySet<ControllableProcessTask>("ControllableProcessTasks");

        return builder.GetEdmModel();
    }
}
