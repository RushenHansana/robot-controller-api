using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using robot_controller_api.Authentication;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ashen Tharuka Desan Fernando",
            Email = "s222448082@deakin.edu.au"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", default);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
});



var app = builder.Build();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup =>
{
    setup.InjectStylesheet("/styles/theme-flattop.css");

});

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();



app.Run();
