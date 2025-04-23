using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Attributes;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh;

internal class Program {
    public static void Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        IEdmModel model = TlharghModelBuilder.GetEdmModel();
        builder.Services
               .AddControllers(opt => opt.Filters.Add<DvinExceptionFilterAttribute>())
               .AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                   .AddRouteComponents("", model)
                   .Conventions.Add(new TlharghConvention())
               );
        builder.Services.UseTlharghDvinAndPegh(Constants.TlharghAppId, new DummyCsArgumentPrompter());

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}