using Charmaran.Api;
using Charmaran.Domain.Entities;
using Charmaran.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register Services
builder.Services.RegisterServices(builder.Configuration);

WebApplication app = builder.Build();

// Initialize the database
using (IServiceScope scope = app.Services.CreateScope())
{
	CharmaranDbContext dbContext = scope.ServiceProvider.GetRequiredService<CharmaranDbContext>();
	RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	
	DatabaseInitializer.MigrateDatabase(dbContext);
	DatabaseInitializer.PostMigrationUpdates(dbContext, roleManager);
}

app.UseCors("CorsPolicy");


//Disable the register endpoint
app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/register"))
	{
		context.Response.StatusCode = StatusCodes.Status404NotFound; // Return 404 for Register endpoint
		return;
	}

	await next();
});

app.MapIdentityApi<CharmaranUser>();

app.UseAuthentication();
app.UseAuthorization();

// Add FastEndpoints
app.UseFastEndpoints().UseSwaggerGen();

app.MapControllers();

app.Run();