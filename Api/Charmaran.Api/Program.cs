using Charmaran.Domain.Constants.Identity;
using Charmaran.Domain.Entities;
using Charmaran.FastEndpoints;
using Charmaran.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(policy =>
{
	policy.AddPolicy("OpenCorsPolicy", opts =>
		opts.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod()
	);
});

// Add Data Protection
//https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-8.0#changing-algorithms-with-usecryptographicalgorithms
builder.Services.AddDataProtection()
	.UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
	{
		EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
		ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
	});


//Add Services
builder.Services.AddControllers();
builder.Services.AddFastEndpointServices(builder.Configuration);

//Add Identity
builder.Services.AddAuthorization(options =>
{
	//Admin Only Policy
	options.AddPolicy(PolicyNames._adminPolicy, policy => policy.RequireRole(RoleNames._admin));
				
	//General Access Policy
	options.AddPolicy(PolicyNames._generalPolicy, policy => 
		policy.RequireAssertion(context => context.User.IsInRole(RoleNames._admin) || context.User.IsInRole(RoleNames._user)));
});

builder.Services.AddIdentity<CharmaranUser, IdentityRole>(options =>
	{
		options.Password.RequiredLength = 9;
		options.Password.RequireUppercase = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireDigit = true;
	})
	.AddEntityFrameworkStores<CharmaranDbContext>()
	.AddDefaultTokenProviders()
	.AddApiEndpoints();

WebApplication app = builder.Build();

// Initialize the database
using (IServiceScope scope = app.Services.CreateScope())
{
	CharmaranDbContext dbContext = scope.ServiceProvider.GetRequiredService<CharmaranDbContext>();
	RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	
	DatabaseInitializer.MigrateDatabase(dbContext);
	DatabaseInitializer.PostMigrationUpdates(dbContext, roleManager);
}

app.UseCors("OpenCorsPolicy");

app.MapIdentityApi<CharmaranUser>();
app.UseAuthentication();
app.UseAuthorization();

// Add FastEndpoints
app.UseFastEndpoints().UseSwaggerGen();

app.MapControllers();

app.Run();