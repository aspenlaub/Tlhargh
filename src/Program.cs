using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh;

internal class Program {
    public static void Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.UseTlharghDvinAndPegh("Tlhargh", new DummyCsArgumentPrompter());
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